import { EventEmitter } from "events";

import dispatcher from "../dispatcher";
import * as MangaActions from "../actions/MangaActions";

var assign = require('object-assign');

class MangaStore extends EventEmitter {
  constructor() {
    super()
        this.mangas = [];
        this.followedMangas = [];
        this.mangasUpdates = [];
        this.manga = undefined;
        
        MangaActions.reloadMangas();
        MangaActions.reloadMangasFollowed();
        MangaActions.reloadNewChapters();
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

    MangaActions.reloadMangas();
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

  getManga() {
    return this.manga;
  }

  followManga(manga) {
    var i = this.getIndex(this.mangas, manga);
    if(i > -1) {
      this.mangas[i] = assign({}, this.mangas[i], {followed: true});
      this.emit("change");
    }
  }

  unfollowManga(manga) {
    var i = this.getIndex(this.mangas, manga);
    if(i > -1) {
      this.mangas[i] = assign({}, this.mangas[i], {followed: false});
      this.emit("change");
    }
  }

  markAsRead(manga) {
    var i = this.getIndex(this.mangasUpdates, manga);
    if(i > -1) {
      this.mangasUpdates.splice(i, 1);
      this.emit("change");
    }
  }
  
  getIndex(arr, o) {
    for(var i = 0; i < arr.length; i++) {
      if(o.id == arr[i].id) {
        return i;
      }
    }

    return -1;
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
      case "RECEIVE_FOLLOWED_MANGAS": {
        this.followedMangas = action.mangas;
        this.emit("change");
        break;
      }
      case "RECEIVE_NEW_CHAPTERS": {
        action.mangas.forEach(manga => {
          manga.chapterUpdates = manga.chapters;
        });;
        this.mangasUpdates = action.mangas;
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
      case "MARKASREAD_MANGA": {
        this.markAsRead(action.manga);
      }
      case "RECEIVE_MANGA": {
        this.manga = action.manga;
        this.emit("change");
        break;
      }
    }
  }

}

const mangaStore = new MangaStore;
dispatcher.register(mangaStore.handleActions.bind(mangaStore));

export default mangaStore;
