import random
import time

from azure.iot.device import IoTHubDeviceClient, Message

# The device connection string to authenticate the device with your IoT hub.
# Get it using the Azure CLI:
# az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyNodeDevice --output table

def sendToAzure(device, message):
    try:
        client = IoTHubDeviceClient.create_from_connection_string(device)
        print( "Sending message: {}".format(message))
        message = message.encode('utf8')
        client.send_message(message)        
        print ( "Message successfully sent" )        
    except KeyboardInterrupt:
        print ( "IoTHubBridge sample stopped" )

