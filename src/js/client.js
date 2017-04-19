import React from "react";
import ReactDOM from "react-dom";
import { Router, Route, IndexRoute, hashHistory } from "react-router";

import Profile from "./pages/Profile";
import Layout from "./pages/Layout";
import Settings from "./pages/Settings";
import Mangas from "./pages/Mangas.js";
import MangaDetails from "./pages/MangaDetails.js";
import MangasFollowed from "./pages/MangasFollowed.js";
import MangasUpdates from "./pages/MangasUpdates.js";
import Animes from "./pages/Animes.js";
import AnimesFollowed from "./pages/AnimesFollowed.js";
import AnimesUpdates from "./pages/AnimesUpdates.js";
import AuthService from "./utils/AuthService"
import Login from "./pages/Login/Login";
import Logout from "./pages/Login/Logout";

const app = document.getElementById('app');
 
// const auth = new AuthService(__AUTH0_CLIENT_ID__, __AUTH0_DOMAIN__);
const auth = new AuthService('YAmDw5AUhffZAJoYD1kdFWTp0vA8coXv', 'qualmanddriven.eu.auth0.com');

// validate authentication for private routes
const requireAuth = (nextState, replace) => {
  if (!auth.loggedIn()) {
    replace({ pathname: '/login' })
  }
}

ReactDOM.render(
  <Router history={hashHistory}>
    <Route path="/" component={Layout} auth={auth}>
      <IndexRoute component={Mangas}></IndexRoute>
      <Route path="profile" component={Profile} onEnter={requireAuth}></Route>
      <Route path="settings" component={Settings}></Route>
      <Route path="mangas" component={Mangas}></Route>
      <Route path="mangas/:mangaid" component={MangaDetails}></Route>
      <Route path="mangas/followed" component={MangasFollowed} onEnter={requireAuth}></Route>
      <Route path="mangas/updates" component={MangasUpdates} onEnter={requireAuth}></Route>
      <Route path="animes" component={Animes}></Route>
      <Route path="animes/followed" component={AnimesFollowed} onEnter={requireAuth}></Route>
      <Route path="animes/updates" component={AnimesUpdates} onEnter={requireAuth}></Route>
      <Route path="login" component={Login}></Route>
      <Route path="logout" component={Logout}></Route>
    </Route>
  </Router>,
app);
