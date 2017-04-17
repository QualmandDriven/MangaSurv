import React from "react";

import Anime from "../components/Anime";
import * as AnimeActions from "../actions/AnimeActions";
import AnimeStore from "../stores/AnimeStore";
import SearchBar from "../components/SearchBar"


export default class Animes extends React.Component {
  constructor(props, context) {
    super(props, context);
    this.getAnimes = this.getAnimes.bind(this);
    this.state = {
      animes: AnimeStore.getAll(),
      filterText: '',
      auth: props.auth
    };

    this.filterAnimes = this.filterAnimes.bind(this);
  }

  componentWillMount() {
    AnimeStore.on("change", this.getAnimes);
  }

  componentWillUnmount() {
    AnimeStore.removeListener("change", this.getAnimes);
  }

  getAnimes() {
    this.setState({
      animes: AnimeStore.getAll(),
    });
  }

  reloadAnimes() {
    AnimeActions.reloadAnimes();
  }

  filterAnimes(e) {
    this.setState({
      filterText: e
    });   
  }

  render() {
    const { animes } = this.state;

    return (
      <div>
        <h1>Animes</h1>
        <SearchBar filterText = {this.state.filterText} onUserInput={this.filterAnimes} />

        <div>
          {animes.map((anime) => {
            return anime.name.toUpperCase().indexOf(this.state.filterText.toUpperCase()) >= 0 ? <Anime key={anime.id} {...anime}/> : "";
          })}
        </div>
      </div>
    );
  }
}
