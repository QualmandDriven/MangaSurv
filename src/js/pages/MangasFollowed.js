import React from "react";

import Manga from "../components/Manga";
import * as MangaActions from "../actions/MangaActions";
import MangaStore from "../stores/MangaStore";
import SearchBar from "../components/SearchBar"


export default class MangasFollowed extends React.Component {
  constructor(props, context) {
    super(props, context);
    this.getMangas = this.getMangas.bind(this);
    this.state = {
      mangas: MangaStore.getAllFollowedMangas(),
      filterText: '',
      profile: props.auth.getProfile(),
      auth: props.auth
    };

    props.auth.on('profile_updated', (newProfile) => {
      console.log("followed prifle update");
      this.setState({profile: newProfile})
    })

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
      mangas: MangaStore.getAllFollowedMangas(),
    });
  }

  reloadMangas() {
    if(this.state.auth.loggedIn()) {
      MangaActions.reloadMangasFollowed(this.state.profile.name, this.state.auth.getToken());
    }
  }

  filterMangas(e) {
    this.setState({
      filterText: e
    });   
  }

  render() {
    const { mangas, filterText, profile, auth } = this.state;
    
    return (

      <div>
        { 
          auth.loggedIn() == false ?
            (<div>
              <h2>Login</h2>
              <button bsStyle="primary" onClick={auth.login.bind(this)}>Login</button>
            </div>)
          : null }
          <h1>Mangas</h1>
          <SearchBar filterText = {this.state.filterText} onUserInput={this.filterMangas} />
          <button class="btn-success" onClick={this.reloadMangas.bind(this)}>Refresh</button>
          <div>
            {mangas.map((manga) => {
              return manga.name.toUpperCase().indexOf(this.state.filterText.toUpperCase()) >= 0 ? <Manga key={manga.id} {...manga}/> : "";
            })}
          </div>
      </div>
    );
  }
}
