import { EventEmitter } from "events";

import dispatcher from "../dispatcher";
import * as AnimeActions from "../actions/AnimeActions";

var assign = require('object-assign');

class AnimeStore extends EventEmitter {
  constructor() {
    super()
        this.animes = [];
        this.followedAnimes = [];
        this.animesUpdates = [];
        
        AnimeActions.reloadAnimes();
        AnimeActions.reloadAnimesFollowed();
        AnimeActions.reloadNewEpisodes();
  }

  initAnimes() {

  }

  createAnime(text) {
    const id = Date.now();

    // this.animes.push({
    //   id,
    //   text,
    //   complete: false,
    // });

    AnimeActions.reloadAnimes();
  }

  getAll() {
    return this.animes;
  } 

  getAllFollowedAnimes() {
    return this.followedAnimes;
  }

  getAllAnimesUpdates() {
    return this.animesUpdates;
  }

  followAnime(anime) {
    var i = this.getIndex(this.animes, anime);
    if(i > -1) {
      this.animes[i] = assign({}, this.animes[i], {followed: true});
      this.emit("change");
    }
  }

  unfollowAnime(anime) {
    var i = this.getIndex(this.animes, anime);
    if(i > -1) {
      this.animes[i] = assign({}, this.animes[i], {followed: false});
      this.emit("change");
    }
  }

  markAsRead(anime) {
    var i = this.getIndex(this.animesUpdates, anime);
    if(i > -1) {
      this.animesUpdates.splice(i, 1);
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
      case "CREATE_ANIMES": {
        this.createanime(action.text);
        break;
      }
      case "RECEIVE_ANIMES": {
        this.animes = action.animes;
        this.emit("change");
        break;
      }
      case "RECEIVE_FOLLOWED_ANIMES": {
        this.followedAnimes = action.animes;
        this.emit("change");
        break;
      }
      case "RECEIVE_NEW_EPISODES": {
        action.animes.forEach(anime => {
          anime.episodeUpdates = anime.episodes;
        });;
        this.animesUpdates = action.animes;
        this.emit("change");
        break;
      }
      case "FILTER_ANIMES": {
        this.filterAnimes(action.filter);
        break;
      }
      case "FOLLOW_ANIME": {
        this.followAnime(action.anime);
        break;
      }
      case "UNFOLLOW_ANIME": {
        this.unfollowAnime(action.anime);
        break;
      }
      case "MARKASREAD_ANIME": {
        this.markAsRead(action.anime);
      }
    }
  }

}

const animeStore = new AnimeStore;
dispatcher.register(animeStore.handleActions.bind(animeStore));

export default animeStore;
