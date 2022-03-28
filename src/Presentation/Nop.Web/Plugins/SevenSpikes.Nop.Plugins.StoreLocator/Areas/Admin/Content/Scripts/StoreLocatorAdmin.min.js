var mapWrapperId = 'chooseLocationMap';
var map;
var markers = [];

var latitudeElement;
var longitudeElement;

$(document).ready(function () {
    $("#shop-create-or-edit a[data-tab-name='tab-location']").on('click', function () {
        setTimeout(function () {
            google.maps.event.trigger(map, "resize");
        });
    });

    if (document.getElementById(mapWrapperId) != null) {
        loadMapScript();
    }

    latitudeElement = $('#Latitude');
    longitudeElement = $('#Longitude');

    $(latitudeElement).on('change', moveMarkerOnCoordinatesChange);
    $(longitudeElement).on('change', moveMarkerOnCoordinatesChange);

});

function loadMapScript() {
    var apiKey = $('.shop-resources').attr('data-googleapikey').trim();
    var source = "https://maps.googleapis.com/maps/api/js?v=3.exp&libraries=places&callback=initializeMap";

    if (apiKey) {
        source = source + "&key=" + apiKey;
    }

    var script = document.createElement("script");
    script.src = source;
    document.body.appendChild(script);
}

function getCustomMapStyles() {
    var mapStyles = $('.shop-resources').attr('data-mapstyles');
    var mapStylesJson = '';

    if (!mapStyles) {
        return '';
    }

    try {
        mapStylesJson = JSON.parse(mapStyles);
    }
    catch (e) {
        console.log('Invalid custom map styles value!');
    }

    return mapStylesJson;
}

function initializeMap() {
    var map_canvas = document.getElementById(mapWrapperId);
    var map_options = {
        center: new google.maps.LatLng(40.713081, -74.006287),
        zoom: 18,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        styles: getCustomMapStyles()
    };

    map = new google.maps.Map(map_canvas, map_options);

    google.maps.event.addListenerOnce(map, 'idle', function () {
        placeMarkerByAvailableMethod();
        createMapSearchBox();
    });

    google.maps.event.addListener(map, 'click', function (event) {
        repositionMarker(event.latLng.lat(), event.latLng.lng());
    });
}

function addMarker(latitude, longitude, title, isDraggable) {
    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(latitude, longitude),
        map: map,
        title: title,
        draggable: isDraggable
    });
    markers.push(marker);
    marker.setMap(map);

    map.setCenter(new google.maps.LatLng(latitude, longitude));
    
    google.maps.event.addListener(marker, 'dragend', function (evt) {
        latitudeElement.val(evt.latLng.lat());
        longitudeElement.val(evt.latLng.lng());
    });
}

function setMarkerByAddress(address) {
    if (address != null && address.length > 0) {

        $.ajax({
            cache: false,
            type: "GET",
            url: 'https://maps.google.com/maps/api/geocode/json?address=' + address + '&sensor=false',
            success: function(data) {
                if (data && data.status != 'ZERO_RESULTS' && data.results.length > 0) {
                    repositionMarker(data.results[0].geometry.location.lat, data.results[0].geometry.location.lng);
                }
            }
        });
    }
}

function placeMarkerByAvailableMethod() {

    addMarker("40.713081", "-74.006287", "Shop is here", true);

    var latitude = "0";
    var longitude = "0";
    var pattern = new RegExp(/^-?\d+\.?\d*$/);

    if (latitudeElement.length > 0 && longitudeElement.length > 0 && latitudeElement.val().length > 0 && longitudeElement.val().length > 0 &&
        pattern.test(latitudeElement.val().trim()) && pattern.test(longitudeElement.val().trim())) {
        latitude = latitudeElement.val().trim();
        longitude = longitudeElement.val().trim();

        repositionMarker(latitude, longitude);
        return;
    }
    else if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function(position) {
            repositionMarker(position.coords.latitude, position.coords.longitude);
            return;
        });
    } else {
        var nopAddressElement = $('#customer-nop-address');
        if (nopAddressElement.length > 0 && nopAddressElement.val().length > 0) {
            var nopAddress = nopAddressElement.val();
            setMarkerByAddress(nopAddress);
        }
    }
}

function repositionMarker(lat, lng) {
    markers[0].setPosition(new google.maps.LatLng(lat, lng));
    map.setCenter(new google.maps.LatLng(lat, lng));
    latitudeElement.val(markers[0].position.lat());
    longitudeElement.val(markers[0].position.lng());
}

function moveMarkerOnCoordinatesChange() {
    var lat = latitudeElement.val();
    var lng = longitudeElement.val();
    
    var pattern = new RegExp(/^-?\d+\.?\d*$/);

    if (lat.length > 0 && lng.length > 0) {
        if (pattern.test(lat) && pattern.test(lng)) {
            markers[0].setPosition(new google.maps.LatLng(lat, lng));
            map.setCenter(new google.maps.LatLng(lat, lng));
        }
    }
}

// Sets the map on all markers in the array.
function setAllMap(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    setAllMap(null);
}

// Shows any markers currently in the array.
function showMarkers() {
    setAllMap(map);
}

// Deletes all markers in the array by removing references to them.
function deleteMarkers() {
    clearMarkers();
    markers = [];
}

function createMapSearchBox() {
    $("#shop-address-input").on('keyup keypress', function (e) {
        if (e.which == 13) {
            e.preventDefault();
            return false;
        }
    });

    // Create the search box and link it to the UI element.
    var input = /** @type {HTMLInputElement} */(document.getElementById("shop-address-input"));
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    var searchBox = new google.maps.places.SearchBox(/** @type {HTMLInputElement} */(input));
    input.style.display = "block";

    google.maps.event.addListener(searchBox, 'places_changed', function () {
        var places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        repositionMarker(places[0].geometry.location.lat(), places[0].geometry.location.lng());
        map.setCenter(places[0].geometry.location);
    });
}
