import context
import paho.mqtt.subscribe as subscribe
from AzureBridge import sendToAzure

topics = ['sensors/#']
print("Starting...")

# Subscribes to the topics and sends messages to Azure
def startCollecting():
	while (True):
	    m = subscribe.simple(topics,port=1886, hostname="localhost", retained=False, msg_count=1)
	    sendToAzure(m.topic, m.payload)
	    print(m.topic, m.payload)

try:
	startCollecting()
except ConnectionDroppedError as e:
	print("Connection error occurred while sending to hub. Restarting...")
finally:
	startCollecting()

