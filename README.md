# The Parking Lot Project



Description- 

service which gets an image of an Israeli license plate and returns a
decision whether the vehicle may enter a parking lot, the decision should be
determined by spefic rules.
the service connects to OCR API
https://ocr.space/ocrapi
which is able to detect text from images.
.

Installation-
nuget packages required:
* Dapper- micro-ORM supporting Sqlite.
* Newtonsoft.Json- JSON framework for .net
* System.Data.SQLite.Core- SQLite database engine.	 

Usage- 
Click the button "Upload Vehicle From Device" and select an jpg file. 
you can use images located in "Plates Numbers Images" folder, or other images, for testing.
screenshots could be found in "screenshots" folder
