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
    const animeClass = location.pathname.match(/^\/animes/) ? "active" : "";
    const profileClass = location.pathname.match(/^\/profile/) ? "active" : "";
    const settingsClass = location.pathname.match(/^\/settings/) ? "active" : "";
    //const mangasClass = location.pathname.match(/^\/mangas/) ? "active" : "";
    const mangasFollowedClass = location.pathname.match(/^\/mangas\/followed/) ? "active" : "";
    const mangasUpdatesClass = location.pathname.match(/^\/mangas\/updates/) ? "active" : "";
    const navClass = collapsed ? "collapse" : "";

    return (
      <div class="col-sm-3 col-md-2 sidebar" role="navigation">
        <ul class="nav nav-sidebar">
          <li class={profileClass}><Link to="profile" onClick={this.toggleCollapse.bind(this)}>Profile</Link></li>
          <li><Link to="login" onClick={this.toggleCollapse.bind(this)}>Login</Link></li>
          <li><a href="#">Logout</a></li>
        </ul>
        <ul class="nav nav-sidebar">
          <li class={mangasClass}><Link to="mangas" onClick={this.toggleCollapse.bind(this)}>Mangas</Link></li>
          <li class={mangasFollowedClass}><Link to="mangas/followed" onClick={this.toggleCollapse.bind(this)}>Followed Mangas</Link></li>
          <li class={mangasUpdatesClass}><Link to="mangas/updates" onClick={this.toggleCollapse.bind(this)}>Manga Updates</Link></li>
        </ul>
        <ul class="nav nav-sidebar">
          <li class={animeClass}><Link to="animes" onClick={this.toggleCollapse.bind(this)}>Animes</Link></li>
          <li><a href="">Followed Animes</a></li>
          <li><a href="">New Animes</a></li>
        </ul>
      </div>
    );
  }
}

              // <li class={featuredClass}>
              //   <IndexLink to="/" onClick={this.toggleCollapse.bind(this)}>Todos</IndexLink>
              // </li>
              // <li class={profileClass}>
              //   <Link to="profile" onClick={this.toggleCollapse.bind(this)}>Profile</Link>
              // </li>
              // <li class={settingsClass}>
              //   <Link to="settings" onClick={this.toggleCollapse.bind(this)}>Settings</Link>
              // </li>
              // <li class={mangasClass}>
              //   <Link to="mangas" onClick={this.toggleCollapse.bind(this)}>Mangas</Link>
              // </li>