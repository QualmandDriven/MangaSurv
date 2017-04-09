import React from "react";

import Manga from "../components/Manga";
import * as MangaActions from "../actions/MangaActions";
import MangaStore from "../stores/MangaStore";
import SearchBar from "../components/SearchBar"


export default class Mangas extends React.Component {
  constructor(props, context) {
    super(props, context);
    this.getMangas = this.getMangas.bind(this);
    this.state = {
      mangas: MangaStore.getAll(),
      filterText: '',
      auth: props.auth
    };

    this.filterMangas = this.filterMangas.bind(this);
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
    this.setState({
      filterText: e
    });   
  }

  render() {
    const { mangas } = this.state;

    return (
      <div>
        <h1>Mangas</h1>
        <SearchBar filterText = {this.state.filterText} onUserInput={this.filterMangas} />

        <div>
          {mangas.map((manga) => {
            return manga.name.toUpperCase().indexOf(this.state.filterText.toUpperCase()) >= 0 ? <Manga key={manga.id} {...manga}/> : "";
          })}
        </div>
      </div>
    );
  }
}
