import React from "react";
import ReactDOM from "react-dom";
import { Router, Route, IndexRoute, hashHistory } from "react-router";

import Profile from "./pages/Profile";
import Layout from "./pages/Layout";
import Settings from "./pages/Settings";
import Mangas from "./pages/Mangas.js";

const app = document.getElementById('app');

ReactDOM.render(
  <Router history={hashHistory}>
    <Route path="/" component={Layout}>
      <IndexRoute component={Mangas}></IndexRoute>
      <Route path="profile" component={Profile}></Route>
      <Route path="settings" component={Settings}></Route>
      <Route path="mangas" component={Mangas}></Route>
    </Route>
  </Router>,
app);
