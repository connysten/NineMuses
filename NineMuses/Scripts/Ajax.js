function ajaxGetResponse(action) {
    var promise = new Promise(function (resolve, reject) {
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState != 4) {
                return;
            }

            if (this.status == 200) {
                resolve(this.responseText);
            }
            else {
                reject(false);
            }
        };

        xhttp.open("GET", action, true);
        xhttp.send();
    });

    return promise;
}

function ajaxGetBool(action) {
    console.log(action);
    var promise = new Promise(function (resolve, reject) {
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState != 4) {
                return;
            }

            if (this.status == 200) {
                if (this.responseText === "True") {
                    //console.log("(True) Response: " + this.responseText);
                    resolve(true);
                }
                else if (this.responseText === "False") {
                    //console.log("(False) Response: " + this.responseText);
                    resolve(false);
                }
            }
            else {
                //console.log("Error: " + this.status);
                resolve(false);
            }
        };

        xhttp.open("GET", action, true);
        xhttp.send();
    });

    return promise;
}