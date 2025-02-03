# Rijnkaai Business

## Services
- RijnkaaiService
- SlackService

### RijnkaaiService

Implementation of the IRijnkaaiService interface, is reponsible for getting the closed times for Rijnkaai parking.

#### GetRijnkaaiClosedDates()
We call the API of [Antwerp Rijnkaai Parking website](https://www.slimnaarantwerpen.be/nl/auto-taxi/publieke-parkings/parking-rijnkaai) and use a static UUID to look for the relevant data section.

Then we use a Regex to filter the right data and structure it.


### SlackService

Implementation of the ISlackService interface, is responsible for sending messages to Slack.

#### PostMessageToSlack

Responsible for sending the full Parking Report of each month as a single big message to the Slack Channel by creating a SlackMessage object with SlackBlock objects.

Then we send it to a Slack Webhook of the relevant bot so it will post it to the right channel.

#### PostSingleMessageToSlack

Responsible for sending a single Parking Report of the following day as a Slack Message to the webhook of the bot to post it in a channel.
