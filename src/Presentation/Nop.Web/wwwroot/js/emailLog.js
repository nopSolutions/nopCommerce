$.getJSON('https://ipapi.co/json/', function(data) {
   let ip = data.ip;
   let city = data.city;
   let path = window.location.pathname;
   let url = 'https://rcsfunctions.azurewebsites.net/api/PageHit?code=AfDAxdaDaTzYTAcdANjcHu7mNH09dOsFTHKpt0bKi4pjo4mHJF/f6Q==';
   let payload = JSON.stringify({ip: ip, city: city, path: path});
   $.post(url, payload, function( data ) {
      return;
   });
});