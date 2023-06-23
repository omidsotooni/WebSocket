var ws;
const token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJKV1QgV2ViU29ja2V0IiwiaWF0IjoxNjg3NTEzMTUxLCJleHAiOjE3MTkwNDkxNTEsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcyMTkiLCJzdWIiOiJvbWlkQGV4YW1wbGUuY29tIiwiUm9sZSI6WyJNYW5hZ2VyIiwiUHJvamVjdCBBZG1pbmlzdHJhdG9yIl19.lXTaEQUJOCAsK0iVqpR1ds5emmoPr26ljdAUhEcNf4w";

function connect() {
    let txtUserName = document.getElementById("txtUserName");
    var uri = "wss://localhost:44366/wschat?name=" + txtUserName.value + "&token=" + token;
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

    ws.send(JSON.stringify(
        {
            Reciver: txtReciver.value,
            Message: txtSend.value           
        }));
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
