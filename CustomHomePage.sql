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

DELETE FROM user_quick_links WHERE UserQuickLinkId = 1;

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
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/disney-plus.png","https://www.disneyplus.com/","Disney Plus","Streaming Services");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/weather.png","https://weather.com/","The Weather Channel","Weather");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/open-weather.png","https://openweathermap.org/","Open Weather","Weather");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/github.png","https://github.com/","Github","Developer Platform");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/slack.png","https://slack.com/","Slack","Messaging Platform");

INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/bank-america.png","https://www.bankofamerica.com/","Bank Of America","Banking");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/chase-bank.png","https://www.chase.com/","Chase Bank", "Banking");
INSERT INTO quick_links (ImagePath, URL, Name, Category) VALUES ("icons/ally-bank.png","https://www.ally.com/","Ally Bank","Banking");

/*dont insert into prod the ids will differ*/
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

CREATE TABLE `slideshow_images` (
  `SlideShowImageId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ImagePath` varchar(500) NOT NULL,
  `FolderName` varchar(300) NOT NULL,
  PRIMARY KEY (`SlideShowImageId`)
);

CREATE TABLE `user_slideshow_images`(
  `UserSlideShowImageId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `UserName` varchar(256) NOT NULL,
  `SlideShowImageId` bigint unsigned NOT NULL,
  PRIMARY KEY (`UserSlideShowImageId`),
  KEY `SlideShowImageId` (`SlideShowImageId`),
  CONSTRAINT `user_slideshow_images_ibfk_1` FOREIGN KEY (`SlideShowImageId`) REFERENCES `slideshow_images` (`SlideShowImageId`) ON DELETE CASCADE ON UPDATE CASCADE
);

INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",66);

INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/blake-richard-verdoorn-20063-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/derrick-cooper-411726-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/dino-reichmuth-98982-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/johannes-plenio-629984-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/josep-castells-523198-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/layne-lawson-140382-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/martin-jernberg-253929-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/niko-photos-333391-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/ricardo-gomez-angel-404673-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/riccardo-chiarini-365677-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/sallie-michalsky-50718-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/sebastian-unrau-42537-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/sora-sagano-103742-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/trevor-cole-389921-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/veliko-karachiviev-517684-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/westboundary-photography-chris-gill-60180-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/wil-stewart-18242-unsplash.jpg","nature");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("nature/wp4.jpg","nature");

INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/andrea-reiman-304108-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/david-dibert-51640-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/fezbot2000-318667-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/harald-hofer-214964-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/harm-weustink-247501-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/jamie-street-562280-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/janko-ferlic-659681-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/lisa-h-205626-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/photo-nic-co-uk-nic-224377-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/ray-hennessy-139970-unsplash.jpg","animals");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("animals/scott-walsh-315682-unsplash.jpg","animals");

INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/adrian-schwarz-530523-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/bicad-media-39529-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/dominik-qn-45994-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/illia-cherednychenko-181621-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/jamie-street-136939-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/jesse-orrico-82919-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/jonathan-roger-494633-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/margaret-barley-42-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/pedro-kummel-25432-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/pedro-lastra-152876-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/pedro-lastra-159224-unsplash.jpg","cityskylines");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("cityskylines/zach-farmer-31340-unsplash.jpg","cityskylines");

INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/david-clode-453251-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/david-clode-635942-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/david-clode-744107-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/david-di-veroli-3558-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/erin-simmons-382348-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/frank-mckenna-150516-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/irina-iriser-654436-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/ishan-seefromthesky-798062-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/james-carol-lee-642040-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/jeremy-bishop-93208-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/laura-college-286844-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/michal-mrozek-1238422-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/milos-prelevic-532780-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/wexor-tmg-26886-unsplash.jpg","ocean");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("ocean/zhan-zhang-1095746-unsplash.jpg","ocean");

INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/alexander-andrews-520231-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/brett-ritchie-550788-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/bryan-goff-360297-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/clemente-ruiz-abenza-134559-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/federico-beccari-67272-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/jeremy-thomas-98201-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/jeremy-thomas-99326-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/nasa-45072-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/neven-krcmarek-152344-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/nick-owuor-astro_nic-757202-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/robson-hatsukami-morgan-296510-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/ryan-hutton-37733-unsplash.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/wp1.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/wp2.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/wp3.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/wp5.jpg","space");
INSERT INTO slideshow_images (ImagePath, FolderName) VALUES ("space/wp6.jpg","space");

/*dont insert into prod the ids will differ*/
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",66);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",67);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",68);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",69);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",70);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",71);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",72);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",73);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",74);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",75);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",76);
INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty",77);

SELECT DISTINCT FolderName FROM slideshow_images WHERE SlideShowImageId IN (SELECT SlideShowImageId from user_slideshow_images WHERE UserId = "251d80ae-93a3-401c-9be9-1ef83e30d541");
SELECT DISTINCT FolderName FROM slideshow_images;

INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) VALUES ("251d80ae-93a3-401c-9be9-1ef83e30d541","BeefCakeTheMighty") SELECT SlideShowImageId FROM slideshow_images WHERE FolderName = "nature";


CREATE TABLE `quotes` (
  `QuoteId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Quote` varchar(500) NOT NULL,
  `Author` varchar(100) NULL,
  PRIMARY KEY (`QuoteId`)
);

INSERT INTO quotes (Quote, Author) VALUES ("Out of every one hundred men, ten shouldn't even be there, eighty are just targets, nine are the real fighters, and we are lucky to have them, for they make the battle. Ah, but the one, one is a warrior...", "Heraclitus");
INSERT INTO quotes (Quote) VALUES ("The human brain isn't designed to keep us happy. It's designed to keep us alive!!");
INSERT INTO quotes (Quote) VALUES ("THOSE WHO ARE IGNORANT AND ASLEEP ARE THOSE WHO HAVE CHOSEN TO STAY THAT WAY BECAUSE THEY ALLOW THE COMFORT OF DECEPTION TO MAKE THEM THINK IT'S SAFER!");
INSERT INTO quotes (Quote) VALUES ("Seek progress, not perfection. As someone who has extremely high standards for myself, I have to repeat this daily.");
INSERT INTO quotes (Quote) VALUES ("You have a limited amount of energy and time in any given day - and you get to choose where you place that energy. Think of it like chips at a roulette table. When I feel angry thoughts about a news article, hear an opinion I disagree with, come across an asshole driving on the freeway... all of these things take energy, my chips, which are an extremely limited resource. Place your chips wisely. I stop myself many times a day from using mine now, and I'm much happier.");
INSERT INTO quotes (Quote) VALUES ("A jack of all trades is a master of none, but oftentimes better than a master of one.");