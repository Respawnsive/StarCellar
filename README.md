# StarCellar
A walkthrough tutorial about Apizr, a Refit based web api client manager, but resilient (retry, connectivity, cache, auth, log, priority, etc...).

This repository contains the source code of the StarCellar sample app built during the corresponding end-to-end walkthrough.
Each covered module get a chapter on the YouTube video and a dedicated branch on GitHub with both With and Without Apizr sample project.
The main branch get the full picture with all modules consolidated into the solution.
You can compare main branch's commits to see what has changed between modules, or modules branch's commits to get a more detailed picture.
You can switch between branches to jump between modules within the same opened solution.
Each module begins from the end of the previous one.

Before you jump into this walkthrough, it is highly recommended to take a look at the official documentation of Apizr and its source code:

[![Read - Documentation](https://img.shields.io/badge/read-documentation-blue?style=for-the-badge)](https://apizr.net/ "Go to project documentation") [![Browse - Source](https://img.shields.io/badge/browse-source_code-lightgrey?style=for-the-badge)](https://github.com/Respawnsive/Apizr "Go to project repository")

If you want to get a quick and small picture of it, please head to our Apizr version of JM's [MonkeyFinder sample tutorial](https://www.github.com).

This StarCellar walkthrough aims to dive pretty much deeper into most of Apizr features than the MonkeyFinder one, so take a breath and let's dive!

Here is the module list:

|Modules|Video chapters|Code branches|Doc articles|
|---|---|---|---|
|APIZR001|[Getting started](https://www.youtube.com)|[01-Getting_started](https://github.com/Respawnsive/StarCellar/tree/01-Getting_started)|[Getting started classic](https://www.apizr.net/articles/gettingstarted_classic.html?tabs=tabid-extended)|
|APIZR002|[Adjusting basic options](https://www.youtube.com)|[02-Adjusting_basics](https://github.com/Respawnsive/StarCellar/tree/02-Adjusting_basics)|[Configuring](https://www.apizr.net/articles/config.html)|
|APIZR003|[Configuring logging with any MS Logging provider](https://www.youtube.com)|[03-Configuring_logging](https://www.github.com)|[Logging](https://www.apizr.net/articles/config_logging.html?tabs=tabid-designing), [Logger](https://www.apizr.net/articles/config_logger.html?tabs=tabid-static)|
|APIZR004|[Applying policies with Polly](https://www.youtube.com)|[04-Applying_policies](https://www.github.com)|[Policies](https://www.apizr.net/articles/config_policies.html?tabs=tabid-static)|
|APIZR005|[Checking connectivity with an external plugin](https://www.youtube.com)|[05-Checking_connectivity](https://www.github.com)|[Connectivity](https://www.apizr.net/articles/config_connectivity.html?tabs=tabid-static)|
|APIZR006|[Managing request priorities with Fusillade](https://www.youtube.com)|[06-Prioritizing_requests](https://www.github.com)|[Priority](https://www.apizr.net/articles/config_priority.html?tabs=tabid-designing)|
|APIZR007|[Handling authentication](https://www.youtube.com)|[07-Handling_authentication](https://www.github.com)|[Authentication](https://www.apizr.net/articles/config_auth.html?tabs=tabid-static)|
|APIZR008|[Caching data with Akavache, MonkeyCache or any MS Caching provider](https://www.youtube.com)|[08-Caching_data](https://www.github.com)|[Data caching](https://www.apizr.net/articles/config_datacaching.html?tabs=tabid-inmemory%2Ctabid-static)|
|APIZR009|[Catching exceptions](https://www.youtube.com)|[09-Catching_exceptions](https://www.github.com)|[Exception handling](https://www.apizr.net/articles/config_exceptions.html?tabs=tabid-registering)|
|APIZR010|[Canceling and timing out](https://www.youtube.com)|[10-Canceling_requests](https://www.github.com)|[Cancellation](https://www.apizr.net/articles/config_cancellation.html), [Timeout](https://www.apizr.net/articles/config_timeout.html?tabs=tabid-designing)|
|APIZR011|[Mapping data with AutoMapper or Mapster](https://www.youtube.com)|[11-Mapping_data](https://www.github.com)|[Data mapping](https://www.apizr.net/articles/config_datamapping.html?tabs=tabid-static)|
|APIZR012|[Sending request with MediatR](https://www.youtube.com)|[12-Sending_requests](https://www.github.com)|[MediatR](https://www.apizr.net/articles/config_mediatr.html?tabs=tabid-imediator)|
|APIZR013|[Returning option result with Optional.Async](https://www.youtube.com)|[13-Returning_option](https://www.github.com)|[Optional.Async](https://www.apizr.net/articles/config_optional.html?tabs=tabid-imediator)|
|APIZR014|[Downloading and uploading files with FileTransfer](https://www.youtube.com)|[14-Transfering_files](https://www.github.com)|[File transfer](https://www.apizr.net/articles/config_transfer.html?tabs=tabid-upload%2Ctabid-static%2Ctabid-globally)|
|APIZR015|[Customizing advanced options](https://www.youtube.com)|[15-Customizing_further](https://www.github.com)|[Configuring](https://www.apizr.net/articles/config.html)|
|APIZR016|[Generating all from Swagger with NSwag](https://www.youtube.com)|[16-Generating_all](https://www.github.com)|[NSwag](https://www.apizr.net/articles/tools_nswag.html)|