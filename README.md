# SeniorTest

<h1>Instructions</h1>

<p>The assignment is to implement a system called <b>“InstaShare”</b>.</p>

The frontend (either a web or a mobile app) should be implemented with the latest stable version of the frontend
technology specified by your job application. The frontend should be based on modern UI/UX principles and
methodologies.
The backend should be implemented as an independent JSON-based Web service powered by a database or a
distributed file system (DFS).
Users of the website should be able to create an account and login with a previously created account.
Once logged in, the user should be able to upload a file that will be processed asynchronously by the backend. Once
the file has been uploaded, the user can proceed with uploading more files, review the name, status and size of
previously uploaded files, or change the name of a previously uploaded file.
Uploaded files are stored in a database or a distributed file system and a service job should pick up the file from the
database / DFS, compress it with ZIP and reinsert it into the database / DFS.
Once the file has been zipped, the user of the community site can download the file

<h2>Tecnologies used:</h2>
<ul>
 <li> .Net 6 </li> 
 <li> Blazor Server App</li>
 <li> Syncfusion Blazor (components for the client project)</li>
 <li> Sql Server 2019</li>
 <li> Bearer Authentication with Microsoft Identity</li>
 <li> Refit for Services</li>
 <li> Generic Repository Pattern</li>
 <li> Custom Onion Arquitecture</li>
 </ul>
 
 <h2>Known bugs</h2>
 <ul>
 <li> Files cann't be downloaded due to a httpclient miss configuration for sending the bearer token.</li>
 <li> Imagen thumbnails cann't be seeing due to a httpclient miss configuration for sending the bearer token.</li>
 </ul>
