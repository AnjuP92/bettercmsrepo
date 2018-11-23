window.fbAsyncInit = function () {
    FB.init({
        appId: facebookId,
        status: true,
        cookie: true,
        xfbml: true
    });

    fbLogout();
};


var logout="false";
function fbLogout(){
    FB.getLoginStatus(function(response) {
        if (response.status === 'connected') {
            //document.getElementById('auth-loggedout').style.display = 'block';
            var loginStatusFB = localStorage.getItem('FBloggedIn');
            if (loginStatusFB == 'yes') {

                var link = document.getElementsByClassName("bcms-cp-logout")[0];
                link.setAttribute("onclick", "fbLogoutUser();");

               

                //FB.api('/me', 'get', { fields: 'id,name,gender' }, function (response) {
                //    var news = document.getElementsByClassName("logoutButton")[0];

                //    var div1 = document.createElement("div");
                //    var span1 = document.createElement("span");
                //    span1.setAttribute("id", "Namestyle");
                //    span1.innerHTML = ("Hi " + response.name);
                //    //alert(response.id);
                //    div1.appendChild(span1);
                //    var logoutBtn = document.createElement("button");
                //    logoutBtn.innerText = "sign Out";
                //    logoutBtn.setAttribute("id", "Gsignin");
                //    logoutBtn.setAttribute("onclick", "fbLogoutUser();");
                //    div1.appendChild(logoutBtn);
                //    news.appendChild(div1);
                //    //alert(response.name);
                //});
            }
            else {

            }
            //document.getElementById('appcontainer').style.display = 'block';
         
            //logout=true;
           
        } else if (response.status === 'not_authorized') {
            // the user is logged in to Facebook,
            // but has not authenticated your app
            //document.getElementById('auth-loggedout').style.display = 'none';
            //document.getElementById('appcontainer').style.display = 'none';
        } else {
            // the user isn't logged in to Facebook.
            //document.getElementById('auth-loggedout').style.display = 'none';
            //document.getElementById('appcontainer').style.display = 'none';
        }
    });

    //if (logout === 'false')
    //{
    //    //FB.logout(function (response) {
        //    // user is now logged out
        //    document.location.href = "http://m.facebook.com/logout.php?confirm=1&next=http://bettercms.sandbox.mvc4.local.net/;";

        //    //location.reload();
        //});

        //function fbLogoutUser() {
        //    FB.getLoginStatus(function (response) {
        //        if (response && response.status === 'connected') {
        //            FB.logout(function (response) {
        //                document.location.reload();
        //            });
        //        }
        //    });
        //}
    //}
};

//FB.logout(function (response) {
//    // user is now logged out
//    document.location.href = "http://m.facebook.com/logout.php?confirm=1&next=http://bettercms.sandbox.mvc4.local.net/;";

//    //location.reload();
//});

function fbLogoutUser() {
    FB.getLoginStatus(function (response) {
        if (response && response.status === 'connected') {
            FB.logout(function (response) {
                localStorage.removeItem('FBloggedIn');
                //document.location.reload();
                $.ajax({
                    url: "/bcms-root/Authentication/Logout",
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
            });
        }
    });
}

function getDataFromFBAndgetSessionToken() {
    FB.api('/me?fields=id,name,first_name,last_name,username', function (response) {

        var AccessToken = FB.getAuthResponse()['accessToken'];

        userFirstName = response.first_name;

        //var User = new classUser();
        
        //User.set_access_token(varAccessToken);
        //User.set_fb_user_id(response.id);
        //User.set_first_name(userFirstName);
        //User.set_last_name(response.last_name);
        //User.set_user_name(response.username);

       

    });
}




// Load the SDK asynchronously

(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
}(document));
