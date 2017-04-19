import React from "react";

import Manga from "../components/Manga";
import * as MangaActions from "../actions/MangaActions";
import MangaStore from "../stores/MangaStore";
import SearchBar from "../components/SearchBar"


export default class MangaDetails extends React.Component {
  constructor(props, context) {
    super(props, context);
    this.getManga = this.getManga.bind(this);

    MangaActions.getManga(this.props.params.mangaid);
    this.state = {
      manga: MangaStore.getManga(),
      auth: props.auth
    };
  }

  componentWillMount() {
    MangaStore.on("change", this.getManga);
  }

  componentWillUnmount() {
    MangaStore.removeListener("change", this.getManga);
  }

  getManga() {
    this.setState({
      manga: MangaStore.getManga(),
    });
  }

  reloadManga() {
    console.log(this.props.params.mangaid);
    MangaActions.getManga(props.params.mangaid);
    this.getManga();
  }

  render() {
    const manga = MangaStore.getManga();
    console.log(this.state);
    console.log(manga);

    return (
      <div>
        <h1>{ manga ? manga.name : null }</h1>

        <div>
          
        </div>
      </div>
    );
  }
}
