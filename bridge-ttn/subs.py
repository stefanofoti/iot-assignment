#import context
import paho.mqtt.client as mqtt
import os
import json
import base64
from AzureBridge import sendToAzure

#Azure connection string
CONNECTION_STRING_1 = "REPLACE_WITH_AZURE_CONNECTION_STRING"
CONNECTION_STRING_2 = "REPLACE_WITH_AZURE_CONNECTION_STRING"

#Topics of the The Things Network built-in broker
TOPIC_DEV_1 = "REPLACE_WITH_APP_NAME/devices/REPLACE_WITH_DEVICE_ID/up"
TOPIC_DEV_2 = "REPLACE_WITH_APP_NAME/devices/REPLACE_WITH_DEVICE_ID/up"

port = 1883
username = "REPLACE_WITH_APP_NAME"
password = "REPLACE_WITH_APP_PASSWORD"
host = "eu.thethings.network"
clientid = "azure-bridge"
cleansession = True
keepalive = 600
qos = 0

#Callback method on message received
def on_message(mqttc, obj, msg):
	print(msg.topic + " " + str(msg.qos) + " " + str(msg.payload))
	#Get TTN auto generated json for message
	payload_json = json.loads(msg.payload)
	#Decode the payload of the final message from json -- it is the json we are looking for
	message = base64.b64decode(payload_json.get('payload_raw').encode('ascii')).decode('ascii')
	#send to azure
	print("Sending to azure: ", message)
	#Send to the right device
	if msg.topic == TOPIC_DEV_1 : sendToAzure(CONNECTION_STRING_1, message)
	if msg.topic == TOPIC_DEV_2 : sendToAzure(CONNECTION_STRING_2, message)

mqttc = mqtt.Client(clientid,cleansession)

#Set callback method
mqttc.on_message = on_message

mqttc.username_pw_set(username, password)
print("Connecting to "+host+" port: "+ str(port))
mqttc.connect(host, port, keepalive)
mqttc.subscribe(TOPIC_DEV_1, qos)
mqttc.subscribe(TOPIC_DEV_2, qos)
mqttc.loop_forever()

