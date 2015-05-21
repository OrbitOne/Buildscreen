#Orbit One Buildscreen
###View the latest builds from your Visual Studio online and Team Foundation Server projects.

Wouldn’t it be easy if you had a visual summary of all your projects? Our build screen is a useful and easy way to display an organized overview of the build statuses of your projects.

Those of you who are familiar with Visual Studio Online (VSO) or Team Foundation Server (TFS), know that keeping tabs on current projects isn’t an easy task. Each page has to be visited separately, this is a very time-consuming process. At Orbit One a desktop application was used to simplify this. The lack of a VSO implementation and the dated user-interface convinced us to write our own build screen as a web application. This web application has several advantages: setup is one-time process, making it readily available for everyone to use. Furthermore, it’s accessible on every device with a modern browser, this way everyone can take advantage of the application anytime, anywhere. The interface has been designed to be user-friendly, with time-saving features.

<a href="http://www.youtube.com/watch?feature=player_embedded&v=tJ5K4H6Wu_k
" target="_blank"><img src="http://img.youtube.com/vi/tJ5K4H6Wu_k/0.jpg" 
alt="promotional video" width="480" height="360" border="10" /></a>

####Supported statuses
* Green: a succeeded build
* Red: a failed build
* Purple: a stopped build
* Cyan: a not yet started build
* Blue: currently building
* Yellow: a build with failed tests

When a project fails to build successfully, a circle is displayed with additional information. Clicking on this circle will lead to a report of the concerning build. This allows for swift assessment of the problem without losing valuable time.

####Filter
You can perform a simple search operation in the navigation bar, this will search on builddefinition name and on team project name. If you want to have a more advanced filter you can enable advanced filter, click on the "filter options" option and a modal will appear. In this modal you can type in the Team Project Name or the Buildname of the builds you want see, this list is complementary (OR relation). On the bottom of this modal you can filter the builds by age, for example you will only see the builds of the last month. This feature is subtractive to the previous two (AND relation).

![filtering image](https://cloud.githubusercontent.com/assets/9320366/7746302/7fdaca2a-ffb1-11e4-9cf0-29c2a7fa1d21.png)

####Configuration
The configuration page makes it easy to add a Visual Studio Online or Team Foundation Server account.
Just fill in a name of choice, the server URL, a VSO/TFS username and password and that's it. 

![configuration image](https://cloud.githubusercontent.com/assets/9320366/7746303/7fe1e5da-ffb1-11e4-9674-fb066031dadf.png)

>Don't forget, to be able to use a VSO account, you have to enable [alternate credentials](https://www.visualstudio.com/en-us/integrate/get-started/auth/overview)!

>Because of the authentication requirements of Team Foundation Server,  the TFS implementation only works On Premise!

### Windows Authentication

Enabling Windows Authentication to secure your application.

#####On Premise:
Add following XML to your web.config. This denies acces to all anonymous user, thus allowing only user that are logged in to your Active Directory.
```sh
    <authentication mode="Windows" />
    <authorization>
      <deny users="?" />
    </authorization>
```
#####On Azure:

   To enable authentication, go the Configure tab and click 'configure' under the authentication / authorization section.  
   For a detailed tutorial [click here](http://azure.microsoft.com/blog/2014/11/13/azure-websites-authentication-authorization/).


###Requirements

* Visual Studio Online
  * Must enable alternate credentials   
* Team Foundation Server
  * Tested on TFS 2012 Update 3, TFS 2013.
  * Must be hosted On Premise
* Browsers, tested on:
  *   IE 9+
  *   Chrome 29+
  *   Firefox 24+
  *   Opera 21+
  *   Safari 7+
  *   Android Browser 4.2+
  *   Windows Phone Browser 8.1
  *   Safari for iPhone 7+
  *   These versions support all functionality, for optimal viewing experience, please update your browser to the latest version.

###Development

This project was started by two students of HoGent, the university college Ghent on the behalf of [Orbit One] as part of our internship.

Want to contribute? Great! 

The possibilities to improve and extend our web applications are endless.

###Tutorial

Please watch our detailed tutorial for a smooth setup.

<a href="http://www.youtube.com/watch?feature=player_embedded&v=SRwHXUJyNuc
" target="_blank"><img src="http://img.youtube.com/vi/SRwHXUJyNuc/0.jpg" 
alt="tutorial video" width="240" height="180" border="10" /></a>

License
---- 
![alt tag](https://www.gnu.org/graphics/gplv3-88x31.png)

GNU GENERAL PUBLIC LICENSE Version 3

 
This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

For any feedback relating to this software, feel free to contact us at: service@orbitone.com

#### Dependencies

JavaScript/AngularJS:
* [AngularJS] - HTML enhanced for web apps!
* [Isotope] - Filter & sort magical layouts.
* [CryptoJS] - Collection of standard and secure cryptographic algorithms.
* [Moment.js] - Parse, validate, manipulate, and display dates in JavaScript.
* [ngTagsInput] - Highly customizable tags input directive built for the AngularJS framework.
* [ngDialog] - Modals and popups provider for Angular.js applications. 

.NET:
* [Json.NET] - Popular high-performance JSON framework for .NET.
* [Windsor] - Castle Windsor is an Inversion of Control container available for .NET.
* [log4net] - Tool to help the programmer output log statements.
* [ClientDependency Framework] - Framework for managing CSS & JavaScript dependencies.
[AngularJS]:http://angularjs.org
[Isotope]:http://isotope.metafizzy.co/
[CryptoJS]:https://code.google.com/p/crypto-js/
[Moment.js]:http://momentjs.com/
[ngTagsInput]:http://mbenford.github.io/ngTagsInput/
[ngDialog]:http://likeastore.github.io/ngDialog/
[Json.NET]:http://www.newtonsoft.com/json
[Windsor]:http://www.castleproject.org/projects/windsor/
[log4net]:https://logging.apache.org/log4net/
[ClientDependency Framework]:https://github.com/Shazwazza/ClientDependency
[Orbit One]: http://www.orbitone.com/
