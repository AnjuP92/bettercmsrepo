

window.fbAsyncInit = function () {
    FB.init({
        appId: facebookId,
        status: true,
        cookie: true,
        xfbml: true
    });
};

// Load the SDK asynchronously

(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
}(document));


FB.getLoginStatus(function (response) {
    if (response.status === 'connected') {
        //// the user is logged in and has authenticated your
        //// app, and response.authResponse supplies
        //// the user's ID, a valid access token, a signed
        //// request, and the time the access token 
        //// and signed request each expire
        //var uid = response.authResponse.userID;
        //var accessToken = response.authResponse.accessToken;
        //var userInfo = document.getElementById('user-info');
        //FB.api('/me', function (response) {
        //    userInfo.innerHTML = '<img src="https://graph.facebook.com/'
        //  + response.id + '/picture">' + response.name;
        //    button.innerHTML = 'Logout';
        //});
        //button.onclick = function () {
        //    FB.logout(function (response) {
        //        var userInfo = document.getElementById('user-info');
        //        userInfo.innerHTML = "";
        //    });
        //};


    } else if (response.status === 'not_authorized') {
         ////the user is logged in to Facebook, 
         ////but has not authenticated your app
        //button.innerHTML = 'Login';
        //button.onclick = function() {
        //    FB.login(function(response) {
        //        if (response.authResponse) {
        //            FB.api('/me', function(response) {
        //                var userInfo = document.getElementById('user-info');
        //                userInfo.innerHTML = 
        //                      '<img src="https://graph.facebook.com/' 
        //                  + response.id + '/picture" style="margin-right:5px"/>' 
        //                  + response.name;
        //            });  

                } else {
                    // the user isn't logged in to Facebook.


                }
            }, true);


            function login() {
                FB.login(function (response) {
     
                    // handle the response
                    //statusChangeCallback(response);
                    if (response.status === 'unknown') {
                        //location.reload();
                    }
                    else {
                        localStorage.setItem('FBloggedIn', 'yes');
                        var loginStatusFB = localStorage.getItem('FBloggedIn');
                        if (loginStatusFB == 'yes') {

                            FB.api('/me', 'get', { fields: 'id,name,gender' }, function (response) {
                                var profile_id = response.id;
                                $.ajax({
                                    url: "/bcms-users/Authentication/SocialLogin?Socialid=" + profile_id,
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

                                window.location = "http://bettercms.sandbox.mvc4.local.net/";
                              
                            });

                        }

                       


                        //window.location = "http://bettercms.sandbox.mvc4.local.net/";
                    }

                }, { scope: 'public_profile,email' });
            }

            function logout() {
                FB.logout(function (response) {
                    // user is now logged out

                });
            }

           
        