CREATE TABLE `quick_links`(
  `QuickLinkId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ImagePath` varchar(500) NOT NULL,
  `URL` varchar(500) NOT NULL,
  `Name` varchar(300) NOT NULL,
  `Category` varchar(300) NOT NULL,
  PRIMARY KEY (`QuickLinkId`)
);

CREATE TABLE `user_quick_links`(
  `UserQuickLinkId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `UserName` varchar(256) NOT NULL,
  `QuickLinkId` bigint unsigned NOT NULL,
  PRIMARY KEY (`UserQuickLinkId`),
  KEY `QuickLinkId` (`QuickLinkId`),
  CONSTRAINT `user_quick_links_ibfk_1` FOREIGN KEY (`QuickLinkId`) REFERENCES `quick_links` (`QuickLinkId`) ON DELETE CASCADE ON UPDATE CASCADE
);

SELECT QuickLInkId FROM user_quick_links WHERE UserId = "251d80ae-93a3-401c-9be9-1ef83e30d541";

SELECT * FROM quick_links WHERE QuickLinkId IN (SELECT QuickLInkId FROM user_quick_links WHERE UserId = "251d80ae-93a3-401c-9be9-1ef83e30d541");

SELECT * FROM quick_links WHERE QuickLinkId IN (SELECT QuickLInkId FROM user_quick_links WHERE UserId = "538fed59-838e-4041-b957-7566a480f11e");

DELETE FROM quick_links WHERE QuickLinkId = 1;

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/twitter.png","https://x.com/","X","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/pinterest.png","https://www.pinterest.com/","Pinterest","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/facebook.png","https://www.facebook.com/","Facebook","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/linkedin.png","https://www.linkedin.com/","Linkedin","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/tumblr.png","https://www.tumblr.com/","Tumblr","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/snapchat.png","https://www.snapchat.com/","Snapchat","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/instagram.png","https://www.instagram.com/","Instagram","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/meetup.png","https://www.meetup.com/","Meetup","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/reddit.png","https://www.reddit.com/","Reddit","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/imgur.png","https://imgur.com/","imgur","Social Media");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/9gag.png","https://9gag.com/","9gag","Social Media");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/google.png","https://www.google.com/","Google","Search Engines");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/google_maps.png","https://www.google.com/maps","Google Maps","Search Engines");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/bing.png","https://www.bing.com/","Bing","Search Engines");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/msn.png","https://www.msn.com/","MSN","Search Engines");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/yahoo.png","https://www.yahoo.com/","Yahoo","Search Engines");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/duckduckgo.png","https://duckduckgo.com/","DuckDuckGo","Search Engines");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/brave.png","https://search.brave.com/","Brave","Search Engines");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/gmail.png","https://www.google.com/gmail/","Gmail","Email");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/yahoo_mail.png","https://mail.yahoo.com/","Yahoo Mail","Email");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/amazon.png","https://www.amazon.com/","Amazon","Shopping");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/ebay.png","https://www.ebay.com/","Ebay","Shopping");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/etsy.png","https://www.etsy.com/","etsy","Shopping");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/netflix.png","https://www.netflix.com/","Netflix","Streaming Services");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/hulu.png","https://www.hulu.com/","Hulu","Streaming Services");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/youtube.png","https://www.youtube.com/","Youtube","Streaming Services");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/weather.png","https://weather.com/","The Weather Channel","Weather");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/github.png","https://github.com/","Github","Developer Platform");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/slack.png","https://slack.com/","Slack","Messaging Platform");


INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",1);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",2);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",3);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",4);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",5);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",6);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",7);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",8);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",9);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",10);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",11);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",12);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",13);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",14);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",15);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",16);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",17);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",18);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",19);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",20);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",21);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",22);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",23);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",24);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",25);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",26);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",27);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",28);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",29);


INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("538fed59-838e-4041-b957-7566a480f11e","MightyDuck",6);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("538fed59-838e-4041-b957-7566a480f11e","MightyDuck",7);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("538fed59-838e-4041-b957-7566a480f11e","MightyDuck",8);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("538fed59-838e-4041-b957-7566a480f11e","MightyDuck",9);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("538fed59-838e-4041-b957-7566a480f11e","MightyDuck",10);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("538fed59-838e-4041-b957-7566a480f11e","MightyDuck",11);
INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES ("538fed59-838e-4041-b957-7566a480f11e","MightyDuck",12);
