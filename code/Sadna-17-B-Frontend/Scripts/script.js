const socket = new WebSocket("wss://localhost:7093/ws?username=noam");

socket.onopen = function (event) {
    console.log("WebSocket connection opened");
};

socket.onclose = function (event) {
    console.log("WebSocket connection closed: ", event);
    alert("sdf");
};

socket.onerror = function (error) {
    console.log("WebSocket error: ", error);
};

socket.onmessage = function (event) {
    const message = event.data;
    console.log("WebSocket message: ", message);
    const no = JSON.parse(message)
    console.log(no)


    const notificationElement = document.createElement('div');
    notificationElement.className = 'notification';
    notificationElement.textContent = message;
    document.body.appendChild(notificationElement);

    setTimeout(() => {
        document.body.removeChild(notificationElement);
    }, 5000); // Remove notification after 5 seconds
};
  
