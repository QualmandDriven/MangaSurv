import React from "react";
import { IndexLink, Link } from "react-router";

export default class Nav extends React.Component {
  constructor() {
    super()
    this.state = {
      collapsed: true,
    };
  }

  toggleCollapse() {
    const collapsed = !this.state.collapsed;
    this.setState({collapsed});
  }

  render() {
    const { location } = this.props;
    const { collapsed } = this.state;
    const mangasClass = location.pathname === "/" || location.pathname.match(/^\/mangas$/) ? "active" : "";
    const animeClass = location.pathname.match(/^\/animes$/) ? "active" : "";
    const profileClass = location.pathname.match(/^\/profile/) ? "active" : "";
    const settingsClass = location.pathname.match(/^\/settings/) ? "active" : "";
    const mangasFollowedClass = location.pathname.match(/^\/mangas\/followed/) ? "active" : "";
    const mangasUpdatesClass = location.pathname.match(/^\/mangas\/updates/) ? "active" : "";
    const animesFollowedClass = location.pathname.match(/^\/animes\/followed/) ? "active" : "";
    const animesUpdatesClass = location.pathname.match(/^\/animes\/updates/) ? "active" : "";
    const navClass = collapsed ? "collapse" : "";

    return (
      <div class="sidebar" role="navigation">
        <h4><a href="/">MangaSurv</a></h4>
        <ul class="nav nav-sidebar">
          <li class={profileClass}><Link to="profile" onClick={this.toggleCollapse.bind(this)}><img class="nav-image" alt="Profile" src="images/nav/profile.svg"/><span>Profile</span></Link></li>
          <li><Link to="login" onClick={this.toggleCollapse.bind(this)}><img class="nav-image" alt="Login" src="images/nav/login.svg"/><span>Login</span></Link></li>
          <li><a href="#"><img class="nav-image" alt="Logout" src="images/nav/logout.svg"/> Logout</a></li>
        </ul>
        <ul class="nav nav-sidebar">
          <li class={mangasClass}><Link to="mangas" onClick={this.toggleCollapse.bind(this)}><img class="nav-image" alt="Mangas" src="images/nav/manga.svg"/><span>Mangas</span></Link></li>
          <li class={mangasFollowedClass}><Link to="mangas/followed" onClick={this.toggleCollapse.bind(this)}><img class="nav-image" alt="Followed Manga" src="images/nav/manga_follow.svg"/><span>Followed Mangas</span></Link></li>
          <li class={mangasUpdatesClass}><Link to="mangas/updates" onClick={this.toggleCollapse.bind(this)}><img class="nav-image" alt="Manga Updates" src="images/nav/manga_updates.svg"/><span>Manga Updates</span></Link></li>
        </ul>
        <ul class="nav nav-sidebar">
          <li class={animeClass}><Link to="animes" onClick={this.toggleCollapse.bind(this)}><img class="nav-image" alt="Animes" src="images/nav/anime.svg"/><span>Animes</span></Link></li>
          <li class={animesFollowedClass}><Link to="animes/followed" onClick={this.toggleCollapse.bind(this)}><img class="nav-image" alt="Followed Animes" src="images/nav/anime_follow.svg"/><span>Followed Animes</span></Link></li>
          <li class={animesUpdatesClass}><Link to="animes/updates" onClick={this.toggleCollapse.bind(this)}><img class="nav-image" alt="Anime Updates" src="images/nav/anime_updates.svg"/><span>New Animes</span></Link></li>
        </ul>
      </div>
    );
  }
}