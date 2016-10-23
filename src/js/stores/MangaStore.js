import { EventEmitter } from "events";

import dispatcher from "../dispatcher";

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
            image: "onepiece",
          },
          {
            id: 2,
            name: "Naruto Shippuuden", 
            chapters: 700, 
            followed: false, 
            lastupdate: Date.now(),
            image: "naruto",
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
    }
  }

}

const mangaStore = new MangaStore;
dispatcher.register(mangaStore.handleActions.bind(mangaStore));

export default mangaStore;
