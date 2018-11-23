

var auth21;

function handleClientLoad() {
    // Loads the client library and the auth2 library together for efficiency.
    // Loading the auth2 library is optional here since `gapi.client.init` function will load
    // it if not already loaded. Loading it upfront can save one network request.
    gapi.load('client:auth2', initClient);
}

function initClient() {
    // Initialize the client with API key and People API, and initialize OAuth with an
    // OAuth 2.0 client ID and scopes (space delimited string) to request access.
    gapi.client.init({
        apiKey: googleApiKey,
        discoveryDocs: ["https://people.googleapis.com/$discovery/rest?version=v1"],
        clientId: googleAppId,
        scope: 'profile'
    }).then(function () {
        // Listen for sign-in state changes.
        gapi.auth2.getAuthInstance().isSignedIn.listen(updateSigninStatus);

        // Handle the initial sign-in state.
        updateSigninStatus(gapi.auth2.getAuthInstance().isSignedIn.get());
    });
}

function updateSigninStatus(isSignedIn) {
    // When signin status changes, this function is called.
    // If the signin status is changed to signedIn, we make an API call.
    if (isSignedIn) {
        makeApiCall();
    }
}

function handleSignInClick(event) {
    // Ideally the button should only show up after gapi.client.init finishes, so that this
    // handler won't be called before OAuth is initialized.
    gapi.auth2.getAuthInstance().signIn();
}

//function handleSignOutClick(event) {
//    //gapi.auth2.getAuthInstance().signOut();
//    auth21 = gapi.auth2.getAuthInstance();
//    auth21.disconnect();
//    //$.ajax({
//    //    url: "/bcms-root/Authentication/Logout",
//    //    type: 'POST',
//    //    async: false,

//    //    success: function (response) {
//    //        //alert("yes success");
//    //        //document.location.href = "https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=http://bettercms.sandbox.mvc4.local.net/login";


//    //    },

//    //    error: function (jqXHR, textStatus, err) {
//    //        //alert(textStatus);
//    //////    }


//    //});



//}

function makeApiCall() {
    // Make an API call to the People API, and print the user's given name.
    
    gapi.client.people.people.get({
        resourceName: 'people/me',
        
    }).then(function (response) {
       
        var str = response.result.resourceName;
        var profileId = str.substring(7);
        alert("google.js");
        localStorage.setItem("response", response);
        $.ajax({
            url: "/bcms-users/Authentication/SocialLogin?Socialid=" + profileId,
                        type: 'POST',
                        async: false,
                        dataType: 'json',
                        success: function (response) {
                            status = true;
                            //if (response === 1) status = true;
                        },
                        error: function (jqXHR, textStatus, err) {
                            //alert(textStatus);
                        }
                    });
        localStorage.setItem('GuserLoggedIn', "yes");
        window.location = "http://bettercms.sandbox.mvc4.local.net/";
        //console.log('Hello, ' + response.result.names[0].givenName);
    }, function (reason) {
        console.log('Error: ' + reason.result.error.message);
    });
}
