
$(window).on('load', function() {

	$('.search-area-wrapper form.search-area-form input').on('keypress', function (e) {
		var input = $(this).val();
		if (input.length > 4) {
			console.log('loading suggestion');
			//$('#search-area-suggestions').load("/collections/all-products/?q="+input+ " .r-catalog-right>.product-box-list>.row:first-child");
			//$('#search-area-suggestions').load("/collections/all-products/?q=" + input + " .product-box-list");
			var url = "/umbraco/Surface/HkDynSearch/ShowSearchSuggestions";

			//$('#search-area-suggestions').innerHTML = data;
			//$('#search-area-suggestions').load(data);

			
			$.ajax({
				url: url,
				type: 'POST',
				data: {input},
				dataType: "json",
				beforeSend: function () {
				},
				success: function (data) {
					console.log("succes, data: "+data);
					//$('#search-area-suggestions').innerHTML = data;
					$('#search-area-suggestions').load(data);					
				},
				error: function () {
				}
			})
			

    	} else {
    		$('#search-area-suggestions').html("");
    	}
    	
    	
    	//console.log(input.length);
    });

});            
