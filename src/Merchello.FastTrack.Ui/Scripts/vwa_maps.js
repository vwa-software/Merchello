/* Custom JS HKLiving */
var infos = [];

function initMapjs(searchTerm, settings, alldealers, currentPos) {

	var isInSearch = searchTerm.length;

	var map = new google.maps.Map(document.getElementById('map'), {
		zoom: 1,
		center: { lat: 52.534682, lng: 5.721809 }
	});

	var zoomMarker = [];

	markers = setMarkers(map, alldealers);

	// Location known?
	if (currentPos) {
		if (settings && settings.ZoomToGeoLocation && settings.ZoomToGeoLocationLevel) {
			findNearest(currentPos);
		}
	} else {
		findNearest();
	}


	function findNearest(pos) {


		var displayMarkersCount = 5;

		if (markers.length < displayMarkersCount - 1) {
			displayMarkersCount = markers.length;
		}

		// show 5 markers around current location, 
		// in case of no current location, show all..
		var nearestMarkers = pos ? markers.slice(0, displayMarkersCount) : markers;

		// if current pos available, take this into the bounds
		// not when searching
		if (pos) {

			var image = {
				url: 'https://hkliving.vwa.nu/Media/dealer-marker.png',
				// This marker is 20 pixels wide by 32 pixels high.
				size: new google.maps.Size(36, 50),
				// The origin for this image is (0, 0).
				origin: new google.maps.Point(0, 0),
				// The anchor for this image is the base of the flagpole at (0, 32).
				anchor: new google.maps.Point(13, 04)
			};

			var shape = {
				coords: [1, 1, 1, 20, 18, 20, 18, 1],
				type: 'poly'
			};

			//var marker2 = new google.maps.Marker({
			//	position: pos,
			//	map: map,
			//	title: 'Your location',
			//	icon: image,
			//	shape: shape
			//});

			var infoWindow = new google.maps.InfoWindow({
				disableAutoPan: true
			});
			infoWindow.setPosition(pos);
			infoWindow.setContent(searchTerm || "Your location");
			infoWindow.open(map);

			//if (!isInSearch) {
			//	nearestMarkers.push(marker2);
			//}
		}

		if (nearestMarkers.length > 1) {
			var bounds = getBoundsForMarkers(nearestMarkers);

			// Apply the bounds to map
			map.fitBounds(bounds);
		}
		else if (nearestMarkers.length === 1) {

			map.setCenter(nearestMarkers[0].getPosition());
			map.setZoom(settings.ZoomToGeoLocationLevel);

			window.setTimeout(function () {
				map.setCenter(nearestMarkers[0].getPosition());
				map.setZoom(settings.ZoomToGeoLocationLevel);
			}, 1);

		}
	}


	/**
	* Returns a bounds object for a given list of markers
	* @param  {array} markers      The markers
	* @return {LatLngBounds}       The bounds object
	*/
	function getBoundsForMarkers(markers) {
		var bounds, marker, _i, _len;

		bounds = new google.maps.LatLngBounds();
		for (_i = 0, _len = markers.length; _i < _len; _i++) {
			marker = markers[_i];
			bounds.extend(marker.getPosition());
		}
		return bounds;
	}

}

function setMarkers(map, locations) {

	var marker, i;

	var image = {
		url: 'https://hkliving.vwa.nu/Media/dealer-marker.png',
		// This marker is 20 pixels wide by 32 pixels high.
		size: new google.maps.Size(36, 50),
		// The origin for this image is (0, 0).
		origin: new google.maps.Point(0, 0),
		// The anchor for this image is the base of the flagpole at (0, 32).
		anchor: new google.maps.Point(0, 36)
	};

	var shape = {
		coords: [1, 1, 1, 20, 18, 20, 18, 1],
		type: 'poly'
	};


	var markers = [];

	// skip dummy marker
	for (i = 0; i < locations.length; i++) {

		//map.setCenter(marker.getPosition());
		var locatie = locations[i].Name;
		var url = locations[i].Url;
		var img = locations[i].Image /*.length ? '<img src="' + locations[i].Image + '" border="0" />' : ''*/;
		var adres = locations[i].Adres;
		var tel = locations[i].Tel;
		var land = locations[i].Land;
		var plaats = locations[i].Plaats;
		var postcode = locations[i].Postcode;
		//var website = locations[i].Website; 
		latlngset = new google.maps.LatLng(locations[i].Latitude, locations[i].Longitude);

		marker = new google.maps.Marker({
			//position: myLatLng,,
			position: latlngset,
			map: map,
			icon: image,
			shape: shape
		});

		markers.push(marker);


		var content = '<div id="iw-container"><div class="iw-content"><div class="iw-naw"><div class="iw-adres"><h5>'
			+ locatie + '</h5>' + adres + ',<br />' + postcode + ' ' + plaats + '<br />' + land + '<br />' +
			// Afsluiten naw div
			'</div></div></div></div>';
		//add

		var infowindow = new google.maps.InfoWindow();

		google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {

			return function () {

				/* close the previous info-window */
				closeInfos();

				infowindow.setContent(content);
				infowindow.open(map, marker);

				/* keep the handle, in order to close it on next click event */
				infos[0] = infowindow;

			};
		})(marker, content, infowindow));

	}

	// Add a marker clusterer to manage the markers.
	var markerCluster = new MarkerClusterer(map, markers,
		{ imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

	return markers;
}

function closeInfos() {

	if (infos.length > 0) {

		/* detach the info-window from the marker ... undocumented in the API docs */
		infos[0].set("marker", null);

		/* and close it */
		infos[0].close();

		/* blank the array */
		infos.length = 0;

	}
}


