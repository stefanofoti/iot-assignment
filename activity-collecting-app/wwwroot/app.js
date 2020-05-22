var sensorDataModel = [];

var index = 1;

var sent = 0;

var result = [];

var resultEdge =[];

function send(){
	var xhr = new XMLHttpRequest();
	xhr.open("POST", "http://127.0.0.1:5000/api/endpoint", true);
	xhr.setRequestHeader('Content-Type', 'application/json');
	xhr.send(JSON.stringify(result));
	sent++;
}

function startCloud(){

    //let sensor = new AbsoluteOrientationSensor({frequency: 60});
	let sensor = new Accelerometer({frequency: 20});
    document.getElementById("startbuttoncloud").remove();     // Hide button

	sensor.start();
	document.getElementById("message").innerHTML = "Starting...";

	sensor.onreading = () => {
		console.log("Acceleration along X-axis: " + sensor.x);
		console.log("Acceleration along Y-axis: " + sensor.y);
		console.log("Acceleration along Z-axis: " + sensor.z);
		document.getElementById("m1").innerHTML = "Reading value " + index + "...";
		document.getElementById("m2").innerHTML = "x: " + sensor.x;
		document.getElementById("m3").innerHTML = "y: " + sensor.y;
		document.getElementById("m4").innerHTML = "z: " + sensor.z;
		var item = {
			Id: index,
			X: sensor.x,
			Y: sensor.y,
			Z: sensor.z
		}
		index++;
		result.push(item);
		var remainder = (index-1)%100;
		document.getElementById("m4").innerHTML = "reminder:" + remainder;		
		if(remainder==0){
			document.getElementById("m1").innerHTML = "Sending msg nr." + sent + "...";
			document.getElementById("m2").innerHTML = "";
			document.getElementById("m3").innerHTML = "";
			document.getElementById("m4").innerHTML = "";
			var xhr = new XMLHttpRequest();
			xhr.open("POST", "/api/endpoint", true);
			xhr.setRequestHeader('Content-Type', 'application/json');
			xhr.send(JSON.stringify(result));
			sent++;
			result=[];
		}
	}

	sensor.onerror = event => console.log(event.error.name, event.error.message);

	sensor.onactivate = () => {
	  this.requestUpdate('sensorDataModel');
	};

	this.sensorDataModel.push(sensor);
	this.requestUpdate("sensorDataModel");

}


function startEdge(){

	let sensor = new Accelerometer({frequency: 20});
    document.getElementById("startbuttonedge").remove();     // Hide button

	sensor.start();
	document.getElementById("message").innerHTML = "Starting...";

	sensor.onreading = () => {
		console.log("Acceleration along X-axis: " + sensor.x);
		console.log("Acceleration along Y-axis: " + sensor.y);
		console.log("Acceleration along Z-axis: " + sensor.z);
		document.getElementById("m2").innerHTML = "x: " + sensor.x;
		document.getElementById("m3").innerHTML = "y: " + sensor.y;
		document.getElementById("m4").innerHTML = "z: " + sensor.z;
		var magnitude = Math.sqrt(sensor.x*sensor.x+sensor.y*sensor.y+sensor.z*sensor.z);
		var delta = 0;
		var status = "";
		if(result["length"]>0){
			oldMagnitude=result[result["length"]-1].Magnitude;
			delta=oldMagnitude-magnitude;
			delta=Math.abs(delta);
			if(delta<0.5){
				status="Standing";
			} else if(delta<1.5){
				status="Car";
			} else if(delta<4.5){
				status="Walking";
			} else {
				status="Running";
			}
			console.log("Current status:" + status);
		}
		var item = {
			Id: index,
			Magnitude: magnitude,
			delta: delta,
			status: status
		}
		index++;
		result.push(item);
		var remainder = (index-1)%20;
		if(remainder==0){
			var Standing=0;
			var Car=0;
			var Walking=0;
			var Running=0;
			var finalStatus;
			document.getElementById("m2").innerHTML = "";
			document.getElementById("m3").innerHTML = "";
			document.getElementById("m4").innerHTML = "";
			result.forEach(element => {
				if(element.status=="Standing")Standing++;
				if(element.status=="Car")Car++;
				if(element.status=="Running")Running++;
				if(element.status=="Walking")Walking++;
			});

			let obj={Standing,Car,Walking,Running},
			greatest=Object.values(obj).sort().pop()
			finalStatus = Object.keys(obj).find( k => obj[k] === greatest )
			document.getElementById("m1").innerHTML = "Status:" + status;
			resultEdge.push(status);
			
			if(resultEdge.length%5==0){
				var xhr = new XMLHttpRequest();
				xhr.open("POST", "/api/endpoint/edge", true);
				xhr.setRequestHeader('Content-Type', 'application/json');
				xhr.send(JSON.stringify(resultEdge));
				sent++;
				resultEdge=[];
				result=[];
			}
		}
	}

	sensor.onerror = event => console.log(event.error.name, event.error.message);

	sensor.onactivate = () => {
	  this.requestUpdate('sensorDataModel');
	};

	this.sensorDataModel.push(sensor);
	this.requestUpdate("sensorDataModel");

}