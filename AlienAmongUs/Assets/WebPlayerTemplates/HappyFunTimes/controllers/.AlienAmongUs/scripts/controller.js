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
var alienKillTimerCounter = 0;
var alienKillTimerStart = true;
var goToNextRow = 0;

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

var homePlanets = ["Earth", "Mars", "Mercury", "Saturn", "Pluto", "Jupiter", "Venus", "Uranus", "Neptune"];
var spaceOccupations = ["Space Doctor", "Space Gardener", "Space Marine", "Space Programmer", "Space Pirate", "Space Jammer", "Space Janitor", "Space Chef", "Space Salesman", "Space Ranger"];
var spaceShip = ["USS Jimmy", "USSR Kosmorockit", "United Space Express", "Galactic Taxi Services", "Uber Cosmos", "Spirit Spaceline", "Space Force One", "Centennial Eagle", "SpaceMetro Transit", "USS Pooter"];

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
  //client.sendCmd('move', {
   // x: position.x / target.clientWidth,
   // y: position.y / target.clientHeight,
  //});
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
/*
inputElem.addEventListener('pointermove', function(event) {
  var position = input.getRelativeCoordinates(event.target, event);
  sendMoveCmd(position, event.target);
  event.preventDefault();
});
*/








function hideonstart() {
    document.getElementById("numpad").style.display = "none";
    document.getElementById("waitingForGameStart").style.display = "none";
}

function waitForGameStart() {
    document.getElementById("welcomeScreen").style.display = "none";
    document.getElementById("waitingForGameStart").style.display = "block";

}

function gameStart() {
    document.getElementById("welcomeScreen").style.display = "none";
    document.getElementById("waitingForGameStart").style.display = "none";
    document.getElementById("allTabs").style.display = "block";
    document.getElementById("numpad").style.display = "block";
    var accusationMenu = $('#accusation_menu');
	accusationMenu.html("");
	var numpadElement = document.getElementById("tabParent");
	var instance = M.Tabs.getInstance(numpadElement);
	instance.select('numpadTab');
	document.getElementById("deadMessage").innerHTML = "";
}
var keypadVal = 0;

function closeIDPage() {
    document.getElementById("idPage").style.display = "none";
    document.getElementById("numpad").style.display = "block";
    document.getElementById("allTabs").style.display = "block";
}

var profilePic
function openCamera(pic) {
    //fix this later plx
    //profilePic = pic;

    // make image to load picture
    var img = new Image();
    // call function when done loading
    img.onload = function () {
        // create a 256x256 canvas
        var canvas = document.createElement("canvas");
        canvas.width = 256;
        canvas.height = 256;
        var ctx = canvas.getContext("2d");
        // scale the image using a css "cover" algo
        var aspect = img.width / img.height;
        var dstHeight = 256
        var dstWidth = dstHeight * aspect;
        if (dstWidth < 256) {
            dstWidth = 256;
            dstHeight = dstWidth / aspect;
        }
        var dstX = (256 - dstWidth) / 2;
        var dstY = (256 - dstHeight) / 2;
        ctx.drawImage(img, dstX, dstY, dstWidth, dstHeight);
        // send the image as a dataUrl to theg game
        client.sendCmd('receivePhoto', {
            dataURL: canvas.toDataURL(),
        });
        // tell the browser we're done
        URL.revokeObjectURL(img.src);
    };
    // load the image
    img.src = URL.createObjectURL(pic);
    

    waitForGameStart();

}

function alienKillTimer()
{
  alienKillTimerCounter = new Date().getTime();
}

function alienKillTimerReset()
{
  console.log(new Date().getTime() - alienKillTimerCounter);
  alienKillTimerCounter = 0;
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
            
            if(new Date().getTime() - alienKillTimerCounter > 300)
            {
              client.sendCmd('requestScan', {
                idToScan: parseInt(keypadVal),
                scanType: 1,
              });

            }
            else
            {
              client.sendCmd('requestScan', {
                idToScan: parseInt(keypadVal),
                scanType: 0,
              });
            }
            keypadVal = 0;
            alienKillTimerReset()
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

function accuseUser(accuseID)
{
  client.sendCmd('accuse', {
    idToAccuse: parseInt(accuseID),
  });
}

function CancelScan()
{
  client.sendCmd('requestScan', {
    idToScan: parseInt(-1),
  });
  document.getElementById("waitingForPlayer").style.display = "none";
  document.getElementById("numpad").style.display = "block";
  document.getElementById("allTabs").style.display = "block";
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
	console.log(type);
	console.log(ID);
	console.log(state);
    document.getElementById("idref1").innerHTML = "Your ID is " + ID;
    document.getElementById("idref2").innerHTML = "Your ID is " + ID;
    
   

});

var type;
var state;

client.addEventListener('assignType', function (data) {

    type = data.type;
	console.log(type);
	console.log(ID);
	console.log(state);
	document.getElementById("playerType").innerHTML = "Your species is " + type;
	if (type == "Alien") {
	    document.getElementById("instructions").innerHTML = 'You must kill every human on the station. After entering an ID, if you hold the checkmark button for a moment and then releasing, you poison the targeted human.'

	}
	else {
	    document.getElementById("instructions").innerHTML = 'You must notice the interactions of the people around you '
	}
	if(ID!=0)
	{
		console.log("starting game");
		gameStart();
	}
});

client.addEventListener('assignState', function (data) {
    state = data.state;
	console.log(type);
	console.log(ID);
	console.log(state);
    if (state == "Dead" && ID != 0) {
        document.getElementById("waitingForPlayer").style.display = "none";
        document.getElementById("idPage").style.display = "none";
        document.getElementById("welcomeScreen").style.display = "none";
        document.getElementById("allTabs").style.display = "none";
        document.getElementById("numpad").style.display = "none";
        document.getElementById("info").style.display = "none";
        document.getElementById("accusation_menu").style.display = "none";
        document.getElementById("deadMessage").innerHTML = "YOU ARE DEAD"

    }
});

client.addEventListener('gameOver', function (data) {
    document.getElementById("waitingForPlayer").style.display = "none";
    document.getElementById("idPage").style.display = "none";
    document.getElementById("welcomeScreen").style.display = "none";
    document.getElementById("allTabs").style.display = "none";
    document.getElementById("numpad").style.display = "none";
    document.getElementById("info").style.display = "none";
    document.getElementById("accusation_menu").style.display = "none";

    if (data.humansWin==1) {

        document.getElementById("deadMessage").innerHTML = "HUMANS WIN";
 
    }
    else
        document.getElementById("deadMessage").innerHTML = "ALIENS WIN";


});




client.addEventListener('idRequestCallback', function (data) {//this is when you send a code and the pc says yes this is valid ONLY HAPPENS FOR THE FIRST PERSON TO INTERACT, THE SECOND PERSON goes straight to IDDELIVERY
    if (data.successState == "Success") {
        document.getElementById("scannedName").innerHTML = data.playerName;

        var firstDigit = data.playerID%10;
        var secondDigit = Math.floor(data.playerID/10)%10;
        var thirdDigit = Math.floor(data.playerID/100)-1;
        document.getElementById("homePlanet").innerHTML = "Home Planet: " + homePlanets[thirdDigit];
        document.getElementById("occupation").innerHTML = "Occupation: " + spaceOccupations[secondDigit];
        document.getElementById("arrivedOn").innerHTML = "Arrived On: " +  spaceShip[firstDigit];
        

        document.getElementById("ScannedImage").innerHTML = "<img src=" + data.playerPhoto + ">";


        document.getElementById("waitingForPlayer").style.display = "none";
        document.getElementById("idPage").style.display = "block";

        

    }
    else {

        document.getElementById("waitingForPlayer").style.display = "none";
        document.getElementById("numpad").style.display = "block";
        document.getElementById("allTabs").style.display = "block";

    }

        

});
// Update accusable list
client.addEventListener('targetDelivery', function(cmd) {
	//document.getElementById("button33").innerHTML = "received";

    var accusationMenu = $('#accusation_menu');
    jQuery.each(cmd["_messages"], function(index, player) {
      /*
    var image = new Image();
    image.src = player.playerPhoto;
    currElement.append(image);
    */
    var newDiv = document.createElement('div');
    if(goToNextRow < 4)
    {
      newDiv.setAttribute('style', 'right: 0');
      goToNextRow++;
    }
    else
    {
      goToNextRow = 0;
    }
		var currElement = document.createElement('img');
		currElement.setAttribute('class', 'waves-effect waves-light btn'); 
    currElement.setAttribute('src', player.playerPhoto);
    currElement.setAttribute('style', 'width:128px; height:128px;');
		let pID = player.playerID;
		currElement.setAttribute('onClick', 'accuseUser('+player.playerID+')');
		
		currElement.style.color = player.playerStatus != "Dead" ? "green" : "red";
		accusationMenu.append(currElement);
    var name = document.createElement('p');
    name.innerHTML = player.playerName;
    //currElement.setAttribute('p', player.playerName);
    accusationMenu.append(newDiv);
    newDiv.append(currElement);
    newDiv.append(name);
	})
      console.log("is this thing on");
  });


$(document).ready(function(){
  $('.tabs').tabs();
});

var check = document.getElementById("confirmbutton");
check.addEventListener("mousedown", function () {alienKillTimer();});
check.addEventListener("mouseup", function() {myFunction(88);});