# Nottingham Tran Twitter Checker

This is a demo application I wrote for myself to try a few things out. First I wanted to write something useful using .NET Core on an operating system other than Windows, second I wanted get some earlier notification of any issues on my way to and from work by the local tram system. Fortunately they have a twitter feed and are pretty good at reporting issues, I'm just too forgetful to remember to go and look most of the time, so I thought I'd write something that checked for and sent me an email.

The code is, honestly, a lot messier than I would like as I've been cobbling this together here and there when I get a few minutes, next up will be giving it a refresh and turning it into something a bit more re-usable.

## What components are being used

This is an [Azure Functions](https://azure.microsoft.com/services/functions/) application which operates on a timer trigger. When the timer is reached then the latest tweets from [NET Nottingham Tram](https://twitter.com/nettram) are retrieved and checked for use of the keywords "delay" and "disruption. If these are found then those tweets are saved to a [Cosmos DB](https://azure.microsoft.com/services/cosmos-db/) document collection, and an email is sent out through [SendGrid](https://sendgrid.com/).

## What settings do I need

The following is a the application settings in the `local.settings.json` file needed to run, substitue with your own values.

Twitter consumer keys and secrets can generated through the [Twitter Application Management](https://apps.twitter.com/) portal.

If you have an Azure subscription then you can get create a SendGrid account as a resource, with plans starting from [free](https://azuremarketplace.microsoft.com/en-us/marketplace/apps/SendGrid.SendGrid?tab=PlansAndPrice) it's worth having a look.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=<storage account name>;AccountKey=<storage account key>;EndpointSuffix=core.windows.net",
    "TwitterAuthApi": "https://api.twitter.com/oauth2/token",
    "TwitterConsumerKey": "<consumer key>",
    "TwitterConsumerSecret": "<consumer secret>",
    "TweetStoreEndPoint": "https://<cosmos db name>.documents.azure.com:443/",
    "TweetStoreAccountKey": "<cosmos db key>",
    "DatabaseId": "<cosmos db database id>",
    "CollectionId": "<cosmos db collection id>",
    "SmtpServer": "smtp.sendgrid.net",
    "SmtpPort": "587",
    "SmtpUsername": "<sendgrid username>",
    "SmtpPassword": "<sendgrid password>",
    "FromAddress": "<email address>",
    "ToAddress": "<email address>"
  }
}
```