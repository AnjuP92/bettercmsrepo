



window.onload = checkStatus();

function checkStatus() {
    var loginStatus = localStorage.getItem('GuserLoggedIn');
    if (loginStatus == 'yes') {
        var link = document.getElementsByClassName("bcms-cp-logout")[0];
        link.setAttribute("onclick", "handleSignOutClick()");
        
    }
}




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
        //gapi.auth2.getAuthInstance().isSignedIn.listen(updateSigninStatus);

        // Handle the initial sign-in state.
        //updateSigninStatus(gapi.auth2.getAuthInstance().isSignedIn.get());
    });
}

//function updateSigninStatus(isSignedIn) {
//    // When signin status changes, this function is called.
//    // If the signin status is changed to signedIn, we make an API call.
//    if (isSignedIn) {
//        //makeApiCall();
//    }
//}

//function handleSignInClick(event) {
//    // Ideally the button should only show up after gapi.client.init finishes, so that this
//    // handler won't be called before OAuth is initialized.
//    gapi.auth2.getAuthInstance().signIn();
//}

function handleSignOutClick(event) {
    //gapi.auth2.getAuthInstance().signOut();
    auth21 = gapi.auth2.getAuthInstance();
    auth21.disconnect();
    $.ajax({
        url: "/bcms-root/Authentication/Logout",
        type: 'POST',
        async: false,

        success: function (response) {
            //alert("yes success");
           


        },

        error: function (jqXHR, textStatus, err) {
            //alert(textStatus);
        }


    });



}

