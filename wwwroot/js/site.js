// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var ws;

function connect() {
    let txtUserName = document.getElementById("txtUserName");
    var uri = "wss://localhost:44366/wschat?name=" + txtUserName.value;
    ws = new WebSocket(uri);

    ws.onopen = function () {
        var divshow = document.getElementById("divshow");
        //alert("[open] Your Connection opend . . .");
        divshow.innerHTML += "Dear " + txtUserName.value + " you are connected.";
    }

    ws.onmessage = function (inData) {
        var jsonObject = JSON.parse(inData.data);
        var divshow = document.getElementById("divshow");
        divshow.innerHTML += "<br/>" + jsonObject.Sender + " : " + jsonObject.Message;
    };

    ws.onerror = function (error) {
        alert(`[error]`);
    };
}
function SendFile() {
    var file = document.getElementById("Fileuploder").files[0];
    var FReader = new FileReader();
    FReader.onload = function () {
        ws.send(FReader.result);
    }
    if (file) {
        FReader.readAsArrayBuffer(file);
    }
}
function SendMsg() {
    var txtSend = document.getElementById("txtSend");
    var txtReciver = document.getElementById("txtReciver");
    ws.send(JSON.stringify({ Reciver: txtReciver.value, Message: txtSend.value }));
    txtSend.value = "";
}

function CloseWS() {
    document.getElementById("myForm").style.display = "none";
    ws.close(1000, "close web socket");
    var divshow = document.getElementById("divshow");
    //alert(`[close] Your Connection closed . . .`);
    divshow.innerHTML += "<br/>Disconnect";
}

function openForm() {
    document.getElementById("myForm").style.display = "block";
}
