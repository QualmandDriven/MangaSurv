import React from "react";

import Manga from "../components/Manga";
import * as MangaActions from "../actions/MangaActions";
import MangaStore from "../stores/MangaStore";


export default class Mangas extends React.Component {
  constructor() {
    super();
    this.getMangas = this.getMangas.bind(this);
    this.state = {
      mangas: MangaStore.getAll(),
    };
  }

  componentWillMount() {
    MangaStore.on("change", this.getMangas);
  }

  componentWillUnmount() {
    MangaStore.removeListener("change", this.getMangas);
  }

  getMangas() {
    this.setState({
      mangas: MangaStore.getAll(),
    });
  }

  reloadMangas() {
    MangaActions.reloadMangas();
  }

  filterMangas(e) {
    const filter = e.target.value;
    MangaActions.filterMangas(filter);   
  }

  render() {
    const { mangas } = this.state;
    
    const MangaComponents = mangas.map((manga) => {
        return <Manga key={manga.id} {...manga}/>;
    });

    return (
      <div>
        <input onChange={this.filterMangas.bind(this)}/>
        <button onClick={this.reloadMangas.bind(this)}>Reload!</button>
        <h1>Mangas</h1>
        <div>
          {mangas.map((manga) => {
            return <Manga key={manga.id} {...manga}/>;
          })}
        </div>
      </div>
    );
  }
}
