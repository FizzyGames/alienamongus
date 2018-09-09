/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
"use strict";

// Start the main app logic.
var commonUI = sampleUI.commonUI;
var input = sampleUI.input;
var misc = sampleUI.misc;
var mobileHacks = sampleUI.mobileHacks;
var strings = sampleUI.strings;
var touch = sampleUI.touch;

var globals = {
    debug: false,
  //orientation: "landscape-primary",
};


misc.applyUrlSettings(globals);
mobileHacks.fixHeightHack();
mobileHacks.disableContextMenu();

var ID = 0;
var score = 0;
var statusElem = document.getElementById("gamestatus");
var inputElem = document.getElementById("inputarea");
var colorElem = document.getElementById("display");
var client = new hft.GameClient();

commonUI.setupStandardControllerUI(client, globals);
commonUI.askForNameOnce();
commonUI.showMenu(true);



var randInt = function(range) {
  return Math.floor(Math.random() * range);
};

// Sends a move command to the game.
//
// This will generate a 'move' event in the corresponding
// NetPlayer object in the game.
var sendMoveCmd = function(position, target) {
  client.sendCmd('move', {
    x: position.x / target.clientWidth,
    y: position.y / target.clientHeight,
  });
};

// Sends a request for all the players' data
var sendAccusationRequest = function()
{
	client.sendCmd("accuseListRequest", {});
};

function setAccusationMenu() {
    var accusationMenu = $('#accusation_menu');
	accusationMenu.html("");
	sendAccusationRequest();
}


// Pick a random color
var color =  'rgb(' + randInt(256) + "," + randInt(256) + "," + randInt(256) + ")";
// Send the color to the game.
//
// This will generate a 'color' event in the corresponding
// NetPlayer object in the game.

//client.sendCmd('color', {
//  color: color,
//});
colorElem.style.backgroundColor = color;

// Send a message to the game when the screen is touched
inputElem.addEventListener('pointermove', function(event) {
  var position = input.getRelativeCoordinates(event.target, event);
  sendMoveCmd(position, event.target);
  event.preventDefault();
});







function hideonstart() {
    document.getElementById("numpad").style.display = "none";
    document.getElementById("waitingForGameStart").style.display = "none";
}

function waitForGameStart() {
    document.getElementById("welcomeScreen").style.display = "none";
    document.getElementById("waitingForGameStart").style.display = "block";
    setTimeout(function () {
        gameStart()
    }, 2000);
}

function gameStart() {
    document.getElementById("waitingForGameStart").style.display = "none";
    document.getElementById("allTabs").style.display = "block";
    document.getElementById("numpad").style.display = "block";
}
var keypadVal = 0;

function closeID() {
    document.getElementById("idPage").style.display = "none";
    document.getElementById("numpad").style.display = "block";
}

function openCamera() {
    //fix this later plx
    waitForGameStart();

}



function myFunction(num) {


    if (document.getElementById("numText").innerHTML.includes("Enter")) {
        keypadVal = 0;
        document.getElementById("numText").innerHTML = "";
    }
        
    if (num == 99){
        document.getElementById("numText").innerHTML = "";
        keypadVal = 0;
    }
 
    else if (num == 88) {
        if (keypadVal>99){ 
            document.getElementById("numText").innerHTML = "";
            keypadVal = 0;
            client.sendCmd('requestScan', {
                idToScan: parseInt(keypadVal),
            });
            document.getElementById("waitingForPlayer").style.display = "block";
            document.getElementById("numpad").style.display = "none";
            document.getElementById("allTabs").style.display = "none";
        }

    }
    else if (keypadVal < 100) {
        keypadVal = keypadVal * 10;
        keypadVal += num;
        document.getElementById("numText").innerHTML = keypadVal

    }
        

}

// Update our score when the game tells us.
client.addEventListener('scored', function(cmd) {
  score += cmd.points;
  statusElem.innerHTML = "You scored: " + cmd.points + " total: " + score;
});

client.addEventListener('idDelivery', function (data) {
    idName = data.playerName;
    
});

client.addEventListener('assignID', function (data) {
    ID = data.ID;
});


client.addEventListener('validID', function (data) {//this is when you send a code and the pc says yes this is valid ONLY HAPPENS FOR THE FIRST PERSON TO INTERACT, THE SECOND PERSON goes straight to IDDELIVERY
    if (data.valid) {
        document.getElementById("waitingForPlayer").style.display = "none";
        document.getElementById("numpad").style.display = "block";

    }
    else {

        document.getElementById("waitingForPlayer").style.display = "none";
        document.getElementById("idPage").style.display = "block";
        document.getElementById("allTabs").style.display = "block";

    }

        

});
// Update accusable list
client.addEventListener('targetDelivery', function(cmd) {
	document.getElementById("button33").innerHTML = "received";
    var accusationMenu = $('#accusation_menu');
	jQuery.each(cmd["_messages"], function(index, player) {
		var currElement = document.createElement('a');
		currElement.setAttribute('class', 'waves-effect waves-light btn col s6'); 
		let pID = player.playerID;
		currElement.setAttribute('onClick', 'accuseUser('+player.playerID+')');
		currElement.innerHTML = player.playerName;
		currElement.style.color = player.playerStatus != "Dead" ? "green" : "red";
		accusationMenu.append(currElement);
	})});


$(document).ready(function(){
  $('.tabs').tabs();
});
