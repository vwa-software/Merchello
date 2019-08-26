namespace Merchello.Web.WebApi
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using Core;
    using Core.Configuration;
    using Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.WebApi;

    /// <summary>
    /// Schedule Tasks
    /// </summary>
    [PluginController("Merchello")]
    public class ScheduledTasksApiController : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="IAnonymousCustomerService"/>.
        /// </summary>
        private readonly IAnonymousCustomerService _anonymousCustomerService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTasksApiController"/> class. 
        /// </summary>
        public ScheduledTasksApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTasksApiController"/> class. 
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        public ScheduledTasksApiController(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _anonymousCustomerService = ((ServiceContext)merchelloContext.Services).AnonymousCustomerService;
        }

        /// <summary>
        /// Delete all customers older than the date in the setting
        /// 
        /// GET /umbraco/Merchello/ScheduledTasksApi/RemoveAnonymousCustomers
        ///     /Umbraco/Api/ScheduledTasksApiController/RemoveAnonymousCustomers
        /// </summary>
        /// <returns>
        /// The count of anonymous customers deleted
        /// </returns>
        [AcceptVerbs("GET", "POST")]
        public int RemoveAnonymousCustomers()
        {
            int maxDays = MerchelloConfiguration.Current.AnonymousCustomersMaxDays;
			int count = 0;
						
			DateTime dateFrom = ApplicationContext.DatabaseContext.Database.ExecuteScalar<DateTime>("SELECT MIN(createdate), max(createdate) from [merchAnonymousCustomer]");
			DateTime dateTo = DateTime.Today.AddDays(-maxDays);

			LogHelper.Info<string>(string.Format("RemoveAnonymousCustomers - Remove {0} till {1}", dateFrom.ToShortDateString(), dateTo.ToShortDateString()));

			while (dateFrom < dateTo)
			{

				var anonymousCustomers = _anonymousCustomerService.GetAnonymousCustomersCreatedBefore(dateFrom).ToArray();
				_anonymousCustomerService.Delete(anonymousCustomers);
				count += anonymousCustomers.Count();

				LogHelper.Info<string>(string.Format("RemoveAnonymousCustomers - Removed Count {0} from day {1}", count, dateFrom.ToShortDateString()));

				// Force garbage collection
				GC.Collect();
				System.Threading.Thread.Sleep(1000);

				dateFrom = dateFrom.AddDays(1);
			}

			return count;
        }
    }
}
