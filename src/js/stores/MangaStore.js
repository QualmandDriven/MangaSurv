import { EventEmitter } from "events";

import dispatcher from "../dispatcher";
var assign = require('object-assign');

class MangaStore extends EventEmitter {
  constructor() {
    super()
        this.mangas = MANGAS;
        this.followedMangas = MANGASFOLLOWED;
        this.mangasUpdates = MANGASUPDATES;
  }

  initMangas() {

  }

  createManga(text) {
    const id = Date.now();

    // this.mangas.push({
    //   id,
    //   text,
    //   complete: false,
    // });

    this.emit("change");
  }

  getAll() {
    return this.mangas;
  } 

  getAllFollowedMangas() {
    return this.followedMangas;
  }

  getAllMangasUpdates() {
    return this.mangasUpdates;
  }

  followManga(manga) {
    var i = 0;
    for(i = 0; i < this.mangas.length; i++) {
      if(manga.id == this.mangas[i].id)
        break;
    }

    this.mangas[i] = assign({}, this.mangas[i], {followed: true});

    this.emit("change");
  }

  unfollowManga(manga) {
    
    var i = 0;
    for(i = 0; i < this.mangas.length; i++) {
      if(manga.id == this.mangas[i].id)
        break;
    }

    this.mangas[i] = assign({}, this.mangas[i], {followed: false});

    this.emit("change");
  }

  handleActions(action) {
    switch(action.type) {
      case "CREATE_MANGAS": {
        this.createmanga(action.text);
        break;
      }
      case "RECEIVE_MANGAS": {
        this.mangas = action.mangas;
        this.emit("change");
        break;
      }
      case "FILTER_MANGAS": {
        this.filterMangas(action.filter);
        break;
      }
      case "FOLLOW_MANGA": {
        this.followManga(action.manga);
        break;
      }
      case "UNFOLLOW_MANGA": {
        this.unfollowManga(action.manga);
        break;
      }
    }
  }

}

var MANGAS = [
  {id: 1, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
  {id: 2, name: "Naruto Shippuuden", chapters: 700, followed: false, lastupdate: Date.now(),image: "naruto.jpg",},
  {id: 3, name: "Bleach", chapters: 745, followed: false, lastupdate: 123,image: "bleach.jpg",},
  {id: 4, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
  {id: 5, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
  {id: 6, name: "Naruto Shippuuden", chapters: 700, followed: false, lastupdate: Date.now(),image: "naruto.jpg",},
  {id: 7, name: "Bleach", chapters: 745, followed: false, lastupdate: 123,image: "bleach.jpg",},
  {id: 8, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
  {id: 9, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
  {id: 10, name: "Naruto Shippuuden", chapters: 700, followed: false, lastupdate: Date.now(),image: "naruto.jpg",},
  {id: 11, name: "Bleach", chapters: 745, followed: false, lastupdate: 123,image: "bleach.jpg",},
  {id: 12, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
  {id: 13, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
  {id: 61, name: "Naruto Shippuuden", chapters: 700, followed: false, lastupdate: Date.now(),image: "naruto.jpg",},
  {id: 71, name: "Bleach", chapters: 745, followed: false, lastupdate: 123,image: "bleach.jpg",},
  {id: 81, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
  {id: 91, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
  {id: 21, name: "Naruto Shippuuden", chapters: 700, followed: false, lastupdate: Date.now(),image: "naruto.jpg",},
  {id: 31, name: "Bleach", chapters: 745, followed: false, lastupdate: 123,image: "bleach.jpg",},
  {id: 41, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
  {id: 51, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
  {id: 62, name: "Naruto Shippuuden", chapters: 700, followed: false, lastupdate: Date.now(),image: "naruto.jpg",},
  {id: 72, name: "Bleach", chapters: 745, followed: false, lastupdate: 123,image: "bleach.jpg",},
  {id: 82, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
  {id: 912, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
  {id: 922, name: "Naruto Shippuuden", chapters: 700, followed: false, lastupdate: Date.now(),image: "naruto.jpg",},
  {id: 932, name: "Bleach", chapters: 745, followed: false, lastupdate: 123,image: "bleach.jpg",},
  {id: 942, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
  {id: 992, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
  {id: 911, name: "Naruto Shippuuden", chapters: 700, followed: false, lastupdate: Date.now(),image: "naruto.jpg",},
  {id: 711, name: "Bleach", chapters: 745, followed: false, lastupdate: 123,image: "bleach.jpg",},
  {id: 811, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
];

var MANGASFOLLOWED = [
  {id: 3, name: "Bleach", chapters: 745, followed: true, lastupdate: 123,image: "bleach.jpg",},
  {id: 4, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
];

var MANGASUPDATES = [
  {id: 3, name: "Bleach", chapters: 745, followed: true, lastupdate: 123,image: "bleach.jpg", chapterUpdates: [{id: 12323,  chapter: 767, added: "2016-04-23"}, {id: 22323, chapter: 768, added: "2016-04-29"}],},
  {id: 4, name: "Onepunch-Man", chapters: 250, followed: true, lastupdate: 123,image: "onepunchman.jpg",},
  {id: 992, name: "One Piece", chapters: 745, followed: true, lastupdate: Date.now(),image: "onepiece.jpg",},
];

const mangaStore = new MangaStore;
dispatcher.register(mangaStore.handleActions.bind(mangaStore));

export default mangaStore;
