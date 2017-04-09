import React from "react";
import ReactDOM from "react-dom";
import { Router, Route, IndexRoute, hashHistory } from "react-router";

import Profile from "./pages/Profile";
import Layout from "./pages/Layout";
import Settings from "./pages/Settings";
import Mangas from "./pages/Mangas.js";
import MangasFollowed from "./pages/MangasFollowed.js";
import MangasUpdates from "./pages/MangasUpdates.js";
import AuthService from "./utils/AuthService"
import Login from "./pages/Login/Login";

const app = document.getElementById('app');
 
// const auth = new AuthService(__AUTH0_CLIENT_ID__, __AUTH0_DOMAIN__);
// "AUTH0_CLIENT_ID='HlyqIQq8LEDJxkRYu8lD4gXYWtuw8Tok\nAUTH0_DOMAIN='qualmanddriven.eu.auth0.com'"
// const auth = new AuthService('HlyqIQq8LEDJxkRYu8lD4gXYWtuw8Tok', 'qualmanddriven.eu.auth0.com');
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
      <Route path="mangas/followed" component={MangasFollowed} onEnter={requireAuth}></Route>
      <Route path="mangas/updates" component={MangasUpdates} onEnter={requireAuth}></Route>
      <Route path="login" component={Login}></Route>
    </Route>
  </Router>,
app);
