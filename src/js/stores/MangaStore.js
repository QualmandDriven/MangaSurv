import { EventEmitter } from "events";

import dispatcher from "../dispatcher";
var assign = require('object-assign');

class MangaStore extends EventEmitter {
  constructor() {
    super()
        this.allMangas = [
          {
            id: 1,
            name: "One Piece", 
            chapters: 745, 
            followed: true, 
            lastupdate: Date.now(),
            image: "onepiece.jpg",
          },
          {
            id: 2,
            name: "Naruto Shippuuden", 
            chapters: 700, 
            followed: false, 
            lastupdate: Date.now(),
            image: "naruto.jpg",
          }
        ];
      this.mangas = this.allMangas;
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

  filterMangas(filter) {
    this.mangas = new Array();

    this.allMangas.forEach(function(manga) {
      if(manga.name.toUpperCase().indexOf(filter.toUpperCase()) >= 0) {
        this.mangas.push(manga);
      }
    }, this);
    
    this.emit("change");
  }

  followManga(manga) {
    var i = 0;
    for(i = 0; i < this.allMangas.length; i++) {
      if(manga.id == this.allMangas[i].id)
        break;
    }

    this.allMangas[i] = assign({}, this.allMangas[i], {followed: true});

    this.emit("change");
  }

  unfollowManga(manga) {
    
    var i = 0;
    for(i = 0; i < this.allMangas.length; i++) {
      if(manga.id == this.allMangas[i].id)
        break;
    }

    this.allMangas[i] = assign({}, this.allMangas[i], {followed: false});

    this.emit("change");
  }

  handleActions(action) {
    switch(action.type) {
      case "CREATE_MANGAS": {
        this.createmanga(action.text);
        break;
      }
      case "RECEIVE_MANGAS": {
        this.allMangas = action.mangas;
        this.mangas = this.allMangas;
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

const mangaStore = new MangaStore;
dispatcher.register(mangaStore.handleActions.bind(mangaStore));

export default mangaStore;
