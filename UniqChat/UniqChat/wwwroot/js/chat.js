"use strict";
document.addEventListener("DOMContentLoaded", function () {

    var jwtToken = getCookie("jwtToken");
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", { accessTokenFactory: () => jwtToken }).build();//Authorization header

    // Disable send button until connection established.
    document.getElementById("sendButton").disabled = true;

    //GETCOOKIE
    function getCookie(name) {
        var value = "; " + document.cookie;
        var parts = value.split("; " + name + "=");
        if (parts.length === 2) return parts.pop().split(";").shift();
    }
    //SAVEGLOBAL CHAT TO DB
    async function saveGlobalChatMessage(senderUsername, content, timestamp) {
        try {
            const response = await fetch('/api/GlobalChat', {
                method: 'POST',headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({senderUsername: senderUsername,content: content,timestamp: timestamp})});
            if (response.ok) {
                console.log('Global chat message saved successfully.');
            } else {
                console.error('Failed to save global chat message.');
            }
        } catch (error) {
            console.error('Error saving global chat message:', error);
        }
    }

    // Event handler for receiving global chat messages
    connection.on("ReceiveMessage", function (user, message, sentAt) {
     saveGlobalChatMessage(user, message, sentAt);
    });

    // Event handler for receiving direct messages
    connection.on("ReceiveDirectMessage", function (user, message, sentAt) {
        var li = document.createElement("li");
        document.getElementById("messagesList").appendChild(li);
        const formattedTimestamp = formatTimestamp(sentAt);
        li.textContent = `${formattedTimestamp} : ${user} sent : ${message}`;
    });

    //CONNECTION START
    connection.start().then(function () {
        document.getElementById("sendButton").disabled = false;
        }).catch(function (err) {
            console.error(err.toString());
        });

    //GLOBAL CHAT SEND TO ALL CLEINTS
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var user = document.getElementById('loggedUsername').textContent;
        var messageText = tinymce.get("messageInput").getContent();
        var message = $("<div>").html(messageText).text();
        connection.invoke("SendMessage", user, message)
            .catch(function (err) {
                console.error(err.toString());
            });
        event.preventDefault();
    });

    //SEND TO CLIENT
    document.getElementById("SendToClient").addEventListener("click", function (event) {
        var selectedDropdownItem = document.querySelector("#userList .user-item.selected");
        if (selectedDropdownItem) {
            var selectedUsername = selectedDropdownItem.textContent; 
            // Retrieve ConnectionId using AJAX request
            fetch(`/api/LoggedUserApi/GetConnectionId?username=${encodeURIComponent(selectedUsername)}`)
                .then(response => response.json())
                .then(data => {
                    var userId = data.connectionId;
                    var messageText = tinymce.get("messageInput").getContent();
                    var message = $("<div>").html(messageText).text();
                    var SenderUsername = document.getElementById("loggedUsername").textContent;

                    connection.invoke("SendMessageToUser", SenderUsername, selectedUsername, userId, message)
                        .catch(function (err) {
                            console.error(err.toString());
                        });
                })
                .catch(error => {
                    console.error("Error retrieving ConnectionId:", error);
                });
        }
    });

    //TIMESTAMP FROMAT 
    function formatTimestamp(timestamp) {
        const date = new Date(timestamp);
        const formattedDate = new Intl.DateTimeFormat('en', {
            hour: '2-digit',
            minute: '2-digit'
        }).format(date);
        return formattedDate;
    }

    //UPDATE USER LIST , ONLINE USERS
    function updateConnectedUsersDropdown(users) {
        var userList = document.getElementById("userList");
        userList.innerHTML = ""; // Clear the list

        if (users.length === 0) {
            var noUsersItem = document.createElement("li");
            noUsersItem.classList.add("user-item");
            noUsersItem.textContent = "No Users";
            userList.appendChild(noUsersItem);
        } else {
            users.forEach(function (user) {
                var userItem = document.createElement("li");
                userItem.classList.add("user-item");
                userItem.textContent = user;
                userItem.addEventListener("click", function () {
                    selectUser(userItem);
                    CreateUserChatRoom(user);
                    
                    document.getElementById("selectedUserDisplay").textContent = user;
                });
                userList.appendChild(userItem);
            });
        }
    }

    //USER LIST CLICK, CREATE CHAT ROOM
    async function CreateUserChatRoom(user) {
        try {
            var recieverUsername = user;
            var senderUsername = document.getElementById("loggedUsername").textContent;

            const response = await fetch(`/api/UserChat/GetChatHistory?senderUsername=${senderUsername}&receiverUsername=${recieverUsername}`);
            if (response.ok) {
                const messages = await response.json();

                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = "";

                messages.sort((a, b) => new Date(a.timestamp) - new Date(b.timestamp));

                messages.forEach(message => {
                    const messageItem = document.createElement("li");
                     const formattedTimestamp = formatTimestamp(message.timestamp);
                    messageItem.textContent = `${formattedTimestamp}: ${message.senderUsername} sent : ${message.content}`;
                    messagesList.appendChild(messageItem);
                });
            } else {
                console.error('Failed to fetch chat history.');
            }
        } catch (error) {
            console.error('Error fetching chat history:', error);
        }
    }

    //SELECT USER IN USER LIST
    function selectUser(selectedItem) {
        var userItems = document.getElementsByClassName("user-item");
        for (var i = 0; i < userItems.length; i++) {
            userItems[i].classList.remove("selected");
        }
        selectedItem.classList.add("selected");
    }
  
    // Retrieve and update the connected usernames
    connection.on("UpdateConnectedUsers", function (users) {
        updateConnectedUsersDropdown(users);
    });

    // Handle potential connection errors
    connection.onclose(function (error) {
        console.error("Connection closed with error:", error);
    });

});
