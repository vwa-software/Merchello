﻿namespace Merchello.FastTrack.Controllers.Membership
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Net.Mail;
	using System.Web.Mvc;
	using System.Web.Security;
	using Connect.Utils;
	using ETC.B2B.WEB.Models.Member;
	using ETC.B2B.WEB.Services;
	using Merchello.Core;
	using Merchello.Core.Gateways.Notification.Smtp;
	using Merchello.Core.Logging;
	using Merchello.Core.Models;
	using Merchello.FastTrack.Factories;
	using Merchello.FastTrack.Models;
	using Merchello.FastTrack.Models.Membership;
	using Merchello.Web.Controllers;
	using Merchello.Web.Store.Controllers;
	using Merchello.Web.Store.Models;

	using Umbraco.Core;
	using Umbraco.Core.Logging;
	using Umbraco.Core.Services;
	using Umbraco.Web;
	using Umbraco.Web.Models;
	using Umbraco.Web.Mvc;

	using LoginModel = Merchello.FastTrack.Models.Membership.LoginModel;

	/// <summary>
	/// A controller responsible for rendering and handling membership operations.
	/// </summary>
	/// <remarks>
	/// This controller is included for example purposes.  It is very likely that membership requirements
	/// for store implementations will vary.
	/// </remarks>
	[PluginController("FastTrack")]
	public class CustomerMembershipController : CustomerMembershipControllerBase
	{
		/// <summary>
		/// The <see cref="IMemberService"/>.
		/// </summary>
		private readonly IMemberService _memberService;


		/// <summary>
		/// Initializes a new instance of the <see cref="CustomerMembershipController"/> class.
		/// </summary>
		public CustomerMembershipController()
			: this(new NewMemberModelFactory<NewMemberModel>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomerMembershipController"/> class. 
		/// </summary>
		/// <param name="newMemberModelFactory">
		/// The new member model factory.
		/// </param>
		public CustomerMembershipController(NewMemberModelFactory<NewMemberModel> newMemberModelFactory)
		{
			Mandate.ParameterNotNull(newMemberModelFactory, "newMemberModelFactory");
			NewMemberModelFactory = newMemberModelFactory;

			_memberService = ApplicationContext.Current.Services.MemberService;

			this.BillingAddressFactory = new FastTrackBillingAddressModelFactory();
			this.ShippingAddressFactory = new FastTrackShippingAddressModelFactory();
		}

		/// <summary>
		/// Gets the <see cref="FastTrackBillingAddressModelFactory"/>.
		/// </summary>
		protected FastTrackBillingAddressModelFactory BillingAddressFactory { get; private set; }

		/// <summary>
		/// Gets the <see cref="FastTrackShippingAddressModelFactory"/>.
		/// </summary>
		protected FastTrackShippingAddressModelFactory ShippingAddressFactory { get; private set; }

		/// <summary>
		/// Gets the <see cref="NewMemberModelFactory{TModel}"/>.
		/// </summary>
		protected NewMemberModelFactory<NewMemberModel> NewMemberModelFactory { get; private set; }

		/// <summary>
		/// Handles the membership login operation.
		/// </summary>
		/// <param name="model">
		/// The <see cref="Models.Membership.LoginModel"/>.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[HttpPost]
		[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
		//[ValidateAntiForgeryToken]
		public virtual ActionResult Login(LoginModel model)
		{

			// When the user is loged in in another browser window, the antiforgerytoken execption is raised
			// to avoid this, the user is redirected to their account page.
			if (User.Identity.IsAuthenticated)
			{
				return model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
				Redirect("~/") : Redirect(model.SuccessRedirectUrl);
			}

			// this does the same as the ValidateAntiForgeryToken.
			System.Web.Helpers.AntiForgery.Validate();


			if (!ModelState.IsValid) return CurrentUmbracoPage();
			var viewData = new StoreViewData { Success = false };

			model.Username = model.Username.Trim();
			model.Password = model.Password.Trim();

			bool logedIn = false;
			try
			{
				logedIn = Members.Login(model.Username, model.Password);
			}
			catch (System.FormatException)
			{
				// Calculated error, import does not import passwords, so the base-64 string is not recognized.
				// Logger.Error<CustomerMembershipController>("Password for user " + model.Username + " could not be validated", ex);

				var messages = new List<string>
					{
						"Your account does not have a valid password, please use the forgot password link."
					};

				viewData.Messages = messages;
				ViewData["MerchelloViewData"] = viewData;
				return CurrentUmbracoPage();
			}
			catch (Exception ex)
			{

				Logger.Error<CustomerMembershipController>("Password for user " + model.Username + " could not be validated", ex);

				var messages = new List<string>
					{
						"Your account does not have a valid password, please use the forgot password link."
					};
				viewData.Messages = messages;
				ViewData["MerchelloViewData"] = viewData;
				return CurrentUmbracoPage();
			}

			if (logedIn)
			{
				if (model.RememberMe && Response.Cookies[FormsAuthentication.FormsCookieName] != null)
					Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddMonths(2);
			}

			if (!logedIn)
			{
				var member = Members.GetByUsername(model.Username);


				if (member == null)
				{
					viewData.Messages = new[] { "Account does not exist for this email address." };
				}
				else
				{
					var messages = new List<string>
					{
						"Login was unsuccessful with the email address and password entered."
					};

					if (!member.GetPropertyValue<bool>("umbracoMemberApproved")) messages.Add(Umbraco.GetDictionaryValue("Account_disabled"));

					if (member.GetPropertyValue<bool>("umbracoMemberLockedOut"))
					{
						messages.Add("This account has been locked due to too many unsucessful login attempts. <br/>Please contact HKLiving to restore your account");
					}
					else if (!string.IsNullOrWhiteSpace(member.GetPropertyValue<string>("umbracoMemberFailedPasswordAttempts")))
					{
						if (int.TryParse(member.GetPropertyValue<string>("umbracoMemberFailedPasswordAttempts"), out int failedPasswordAttempts))
						{
							messages.Add(string.Format("You have {0} attempts left.", 5 - failedPasswordAttempts));
						}
					}
					viewData.Messages = messages;
				}

				ViewData["MerchelloViewData"] = viewData;
				return CurrentUmbracoPage();
			}

			return model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
				Redirect("~/") : Redirect(model.SuccessRedirectUrl);
		}

		/// <summary>
		/// Handles the account registration.
		/// </summary>
		/// <param name="model">
		/// The <see cref="NewMemberModel"/>.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Register(NewMemberModel model)
		{
			if (!ModelState.IsValid) return CurrentUmbracoPage();

			var logData = new ExtendedLoggerData();
			logData.AddCategory("Merchello");
			logData.AddCategory("FastTrack");

			var registerModel = Members.CreateRegistrationModel(model.MemberTypeAlias);
			registerModel.Name = model.Email;
			registerModel.Email = model.Email;
			registerModel.Password = model.Password;
			registerModel.Username = model.Email;
			registerModel.UsernameIsEmail = true;

			var fn = new UmbracoProperty { Alias = "firstName", Name = "First Name", Value = model.FirstName };
			var ln = new UmbracoProperty { Alias = "lastName", Name = "Last Name", Value = model.LastName };
			registerModel.MemberProperties.Add(fn);
			registerModel.MemberProperties.Add(ln);

			MembershipCreateStatus status;

			//// Add a message for the attempt
			MultiLogHelper.Info<CustomerMembershipController>("Registering a new member", logData);

			var member = Members.RegisterMember(registerModel, out status, model.PersistLogin);

			var registration = NewMemberModelFactory.Create(model, status);

			if (registration.ViewData.Success)
			{
				var membership = _memberService.GetByEmail(model.Email);

				if (member != null)
				{
					_memberService.AssignRole(membership.Id, "Customers");
					_memberService.Save(membership);

					// Log them in 
					Members.Login(registerModel.Username, registerModel.Password);
				}

				var redirectUrl = model.SuccessRedirectUrl.IsNullOrWhiteSpace()
					? Redirect("~/")
					: Redirect(model.SuccessRedirectUrl);

				return redirectUrl;
			}

			// Finally
			ViewData["MerchelloViewData"] = model.ViewData;
			return CurrentUmbracoPage();
		}

		/// <summary>
		/// Logs out the current member.
		/// </summary>
		/// <param name="redirectId">
		/// The Umbraco Id of the page to redirect to after successful logout.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[HttpGet]
		public virtual ActionResult Logout(int redirectId, string path = "")
		{
			Members.Logout();

			if (redirectId == 0 && !string.IsNullOrEmpty(path))
				return Redirect(path);

			if (redirectId == 0)
			{
				return Redirect("/");
			}

			return RedirectToUmbracoPage(redirectId);
		}

		/// <summary>
		/// Responsible for redirecting to the receipt page.
		/// </summary>
		/// <param name="key">
		/// The <see cref="IInvoice"/> Key.
		/// </param>
		/// <param name="redirectId">
		/// The Umbraco page id for the receipt page.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[HttpGet]
		[Authorize]
		public virtual ActionResult ViewReceipt(Guid key, int redirectId)
		{
			if (key.Equals(Guid.Empty)) return Redirect("/");

			// set the invoice key in the cookie (Merchello CustomerContext)
			CustomerContext.SetValue("invoiceKey", key.ToString());

			return RedirectToUmbracoPage(redirectId);
		}

		/// <summary>
		/// Renders the login form.
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
		public virtual ActionResult LoginForm(string view = "", string redirectPath = "")
		{
			var model = new LoginModel { RememberMe = true };
			return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
		}

		/// <summary>
		/// Renders the registration form.
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
		public virtual ActionResult RegisterForm(string view = "")
		{
			var model = NewMemberModelFactory.Create(CurrentCustomer);
			return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
		}

		/// <summary>
		/// Responsible for rendering the Billing Address form.
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
		[Authorize]
		public virtual ActionResult CustomerBillingAddress(string view = "")
		{
			var customer = (ICustomer)CurrentCustomer;
			var caddress = customer.DefaultCustomerAddress(AddressType.Billing);

            //var allAddresses = customer.Addresses;

			var model = caddress != null ?
				this.BillingAddressFactory.Create(customer, caddress) :
				this.BillingAddressFactory.Create(new Address { AddressType = AddressType.Billing });

			return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
		}

		/// <summary>
		/// Responsible for rendering the Shipping Address form.
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
		[Authorize]
		public virtual ActionResult CustomerShippingAddress(string view = "")
		{
			var customer = (ICustomer)CurrentCustomer;
			var caddress = customer.DefaultCustomerAddress(AddressType.Shipping);

			var model = caddress != null ?
				this.ShippingAddressFactory.Create(customer, caddress) :
				this.ShippingAddressFactory.Create(new Address { AddressType = AddressType.Shipping });

			return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
		}

        /// <summary>
		/// Responsible for rendering the Billing Address form.
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
        [Authorize]
        public virtual ActionResult GetAllBillingAddresses(string view = "")
        {
            var customer = (ICustomer)CurrentCustomer;
            var addresses = customer.Addresses.Where(x => x != null && x.AddressType == AddressType.Billing).ToList().Select(a => BillingAddressFactory.Create(customer, a));

            return view.IsNullOrWhiteSpace() ? PartialView(addresses) : PartialView(view, addresses);
        }

        /// <summary>
        /// Responsible for rendering the Shipping Address form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        [Authorize]
        public virtual ActionResult GetAllShippingAddresses(string view)
        {
            var customer = (ICustomer)CurrentCustomer;
            var addresses = customer.Addresses.Where(x => x != null && x.AddressType == AddressType.Shipping).ToList().Select(a => ShippingAddressFactory.Create(customer, a));

            return view.IsNullOrWhiteSpace() ? PartialView(addresses) : PartialView(view, addresses);
        }

        /// <summary>
        /// Handles the membership change password operation.
        /// </summary>
        /// <param name="model">
        /// The <see cref="Models.Membership.ChangePasswordModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public virtual ActionResult ChangePassword(ChangePasswordModel model)
		{
			if (!ModelState.IsValid) return CurrentUmbracoPage();
			var viewData = new StoreViewData();

			model.Password = model.Password.Trim();


			if (!((model.Password.Length >= Membership.MinRequiredPasswordLength) &&
				(model.Password.ToCharArray().Count(c => !char.IsLetterOrDigit(c)) >= Membership.MinRequiredNonAlphanumericCharacters)))
			{
				viewData.Success = false;
				viewData.Messages = new[] { string.Format("New password invalid. Minimum length {0} characters", Membership.MinRequiredPasswordLength) };
				ViewData["MerchelloViewData"] = viewData;
				return CurrentUmbracoPage();
			}

			// change password seems to have a bug that will allow it to change the password even if the supplied 
			// old password is wrong!
			// so use the login to check the old password as a hack
			var currentUser = Membership.GetUser();
			if (!Members.Login(currentUser.UserName, model.OldPassword))
			{
				viewData.Success = false;
				viewData.Messages = new[] { "Current password incorrect." };
				ViewData["MerchelloViewData"] = viewData;
				return CurrentUmbracoPage();
			}

			if (!currentUser.ChangePassword(model.OldPassword, model.Password))
			{
				viewData.Success = false;
				viewData.Messages = new[] { "Change password failed. Please try again." };
				ViewData["MerchelloViewData"] = viewData;
				return CurrentUmbracoPage();
			}

			viewData.Success = true;
			viewData.Messages = new[] { "Password updated successfully" };
			ViewData["MerchelloViewData"] = viewData;
			return CurrentUmbracoPage();
		}

		/// <summary>
		/// Handles the membership forgot password operation.
		/// </summary>
		/// <param name="model">
		/// The <see cref="Models.Membership.ForgotPasswordModel"/>.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult ForgotPassword(ForgotPasswordModel model)
		{
			if (!ModelState.IsValid) return CurrentUmbracoPage();
			var viewData = new StoreViewData();
			var member = this.Services.MemberService.GetByUsername(model.Username);
			if (member == null)
			{
				viewData.Success = false;
				viewData.Messages = new[] { "Unknown user name." };
				ViewData["MerchelloViewData"] = viewData;
				return CurrentUmbracoPage();
			}

			MemberActivationModel activationModel = new MemberActivationModel(member);
			string sEncrypt = Guid.NewGuid().ToString().Replace("-", string.Empty);
			string token = Encrypter.Encrypt(sEncrypt, Encrypter.EncryptEnum.SHA1);
			DateTime dateTime = DateTime.Now;
			dateTime = dateTime.AddDays(60.0);

			string resetTokenExpiryDate = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
			string resetToken = "?token=" + sEncrypt + "&uid=" + member.Username;
			activationModel.ActivationLink = resetToken;
			try
			{
				member.SetValue("passwordResetToken", token);
				member.SetValue("passwordResetTokenExpiryDate", resetTokenExpiryDate.ToString(CultureInfo.InvariantCulture));
				this._memberService.Save(member, true);
			}
			catch (Exception ex)
			{
				LogHelper.Error<CustomerMembershipController>(string.Format("ForgotPassword - documenttype {0} does not contain passwordResetToken property", member.ContentTypeAlias), ex);
			}

			new EmailService().SendMailFromView<MemberActivationModel>(activationModel, "ResetEmailTemplate", "Reset password", new string[] { member.Email });

			viewData.Success = true;
			viewData.Messages = new string[] { "A password reset link has been emailed to you." };

			this.ViewData["MerchelloViewData"] = viewData;
			return (ActionResult)this.CurrentUmbracoPage();

			//var newPassword = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, 0);
			//var user = Membership.GetUser(model.Username);
			//user.ChangePassword(newPassword, newPassword);

			//// assumes you have set the SMTP settings in web.config and supplied a default "from" email
			//var msg = new MailMessage
			//{
			//	Subject = string.Format("New Password for {0}", Request.Url.Host),
			//	Body = string.Format("Your new password is: {0}", newPassword),
			//	IsBodyHtml = false
			//};
			//msg.To.Add(new MailAddress(model.Username));
			//using (var smtpClient = new SmtpClient())
			//{
			//	smtpClient.Send(msg);
			//}

			//viewData.Success = true;
			//viewData.Messages = new[] { "A new password has been emailed to you." };
			//ViewData["MerchelloViewData"] = viewData;
			//return CurrentUmbracoPage();
		}

		/// <summary>
		/// Renders the change password form.
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
		public virtual ActionResult ChangePasswordForm(string view = "")
		{
			var model = new ChangePasswordModel();
			return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
		}

		/// <summary>
		/// Renders the forgot password form.
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
		public virtual ActionResult ForgotPasswordForm(string view = "")
		{
			var model = new ForgotPasswordModel();
			return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
		}
	}
}