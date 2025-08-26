function StartChangePassword() {
    let minPasswordLength = 5;
    //function createSalt() {
    //    const size  = 16;
    //    const allc  = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"; // !@#$%^&*()_+~`|}{[]\:;?><,./-=
    //    let   salt  = '';
    //    for (var i = 0; i < size; i++)
    //        salt += allc[Math.floor(Math.random() * allc.length)];
    //    return salt;
    //}
    function Verify() {
        if (currentPassword.value != "" &&
            currentPassword.value.length <= minPasswordLength) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<div class=\"HTTP Error\">The current password is too short!</div>";
        }
        else if (newPassword1.value != "" &&
            newPassword1.value.length <= minPasswordLength) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<div class=\"HTTP Error\">The new password is too short!</div>";
        }
        else if (newPassword2.value != "" &&
            newPassword2.value.length <= minPasswordLength) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<div class=\"HTTP Error\">The new password2 is too short!</div>";
        }
        else if (newPassword1.value != newPassword2.value) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<div class=\"HTTP Error\">The given passwords do not match!</div>";
        }
        else {
            responseDiv.style.display = 'none';
            responseDiv.innerHTML = "";
        }
        saveButton.disabled = !(responseDiv.innerHTML == "" &&
            currentPassword.value != "" &&
            newPassword1.value != "" &&
            newPassword2.value != "");
    }
    function SaveNewPassword() {
        HTTPSet("/users/" + SignInUser + "/password", {
            "currentPassword": currentPassword.value,
            "newPassword": newPassword1.value
        }, (HTTPStatus, ResponseText) => {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully updated your password.</div>";
        }, (HTTPStatus, StatusText, ResponseText) => {
            var responseJSON = { "description": "HTTP Error " + HTTPStatus + " - " + StatusText + "!" };
            if (ResponseText !== null && ResponseText != "") {
                try {
                    responseJSON = JSON.parse(ResponseText);
                }
                catch (_a) { }
            }
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Updating your password failed!" + (responseJSON.description !== null ? responseJSON.description : "") + "</div>";
        });
    }
    const changePasswordInfos = document.getElementById('changePasswordInfos');
    const changePassword = document.getElementById('changePassword');
    const currentPassword = changePassword.querySelector('#currentPassword');
    const newPassword1 = changePassword.querySelector('#newPassword1');
    const newPassword2 = changePassword.querySelector('#newPassword2');
    const responseDiv = document.getElementById("response");
    const lowerButtonsDiv = changePasswordInfos.querySelector('#lowerButtons');
    const saveButton = lowerButtonsDiv.querySelector("#saveButton");
    currentPassword.onchange = () => { Verify(); };
    currentPassword.onkeyup = () => { Verify(); };
    currentPassword.onchange = () => { Verify(); };
    currentPassword.onkeyup = () => { Verify(); };
    newPassword1.onchange = () => { Verify(); };
    newPassword1.onkeyup = () => { Verify(); };
    newPassword2.onchange = () => { Verify(); };
    newPassword2.onkeyup = () => { Verify(); };
    saveButton.onclick = () => { SaveNewPassword(); };
}
//# sourceMappingURL=password.js.map