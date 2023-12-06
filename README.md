# Driving Routes
This project is a practical part of dipolma thesis **Mapping the trajectory of an object during playback of a geotagged video**

**Programing language: C#, XAML**


## Description
A driving exam consists of the two parts: theory and practical part. Druing the pracital part of the exam, there are routes where candidate drives a car and do the exam. Routes are consisted of traffic elements, such as semaphores and roundabaouts, and paths which connect those elements. In the past, the routes were added manually and that job was difficult. Idea behind this project is to automate that process and make it easier. That requires implementing following functionalities:
* Adding routes
* Finding routes
* Deleting routes
* Following current geographical location of the candidate

### Adding routes
Traffic elements and paths are loaded from the XML file. At the beginning, only elements are show on the map. (Note: There are only 2 types of elements: semaphores and roundabouts). An User clicks elements which are wanted to be included into the route. Also, there are input field which is filled with the name of the route. There are three conditions which new route must satisfy to be added: 
1. The route must have at least 2 different elements. Example: there can be route with one semaphore and one roundabout. However there musn't be a route with 5 or 6 semaphores.
2. The route with the same choosen elements musn't exist.
3. There is no existing route with the same name.

Route will be shown after successful addition, with followng informations about the lenght of the route and its name. Additionally, it will be added into saved routes.

### Finding routes
After loading routes, the user can choose the route for showing by selecting the name of the route from combobox. Additionally, there are options for showing the shortest route, the longest route and random selection of the route.  

### Deleting routes
The user can delete route by selecting its name in the combobox. After deleting, the list of routes in combobox will be updated. Additionally, user can retrieve last deleted route, which is similar to the "Undo" operation.   

### Following current geographical location of the candidate
During the practical driving exam, there si possibility that exam is recorded by a camera. Some cameras have ability to store current geographical coordinates of every second in the video in their metadata. That information is extracted from metadata and used in the project. During playback of the video, geographical location of the candidate is updated in real time, depending of geographical coordinates in that second of the video. Candidate's location is shown as the marker on the map. 
Note: Because video is recorded by IPhone, information about coordinates in every second of the video had to be added mannualy. Some cameras, such as GoPro, have those information in their metadata, which can be extracted automatically, used appropriate tools. (EXIFTool)

