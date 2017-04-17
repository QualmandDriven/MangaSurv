import React from "react";

import * as AnimeActions from "../actions/AnimeActions";
var moment = require("moment");

export default class Anime extends React.Component {
  constructor(props, context) {
      super(props, context);
      this.state = {
        auth: props.auth
      };
  }

  followAnime() {
    AnimeActions.followAnime(this.props);
  }

  unfollowAnime() {
    AnimeActions.unfollowAnime(this.props);
  }

  markAsRead() {
    AnimeActions.markAsRead(this.props);
  }

  render() {

    // const { id, name, episodes, followed, lastupdate, image, episodeUpdates } = this.props;
    // const imagePath = "images/" + this.props.image;
    const imagePath = "images/" + this.props.fileSystemName.replace(/[ :]/g, "_") + ".jpg";

    return (
      <div class={ this.props.episodeUpdates ? "overview overviewWide" : "overview " }>
        <div class={ this.props.episodeUpdates ? "divHalfHorizontal overviewThumb" : "overviewThumb" }>
          {
            this.props.followed ? 
            <button class="btn-danger hoverdeleteoverview" onClick={this.unfollowAnime.bind(this)}>- Unfollow</button>
            :
            <button class="btn-success hoveraddoverview" onClick={this.followAnime.bind(this)}>+ Follow</button> 
          }
          <a href="#">
            <img src={imagePath}/>
          </a>
          <div>
            <p>{ this.props.name }</p>
            {/*<table>
              <thead>
                <tr>
                  <th colSpan="3">{ this.props.name }</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>Episodes:</td>
                  <td>{ this.props.episodes.length }</td>
                </tr>
                <tr>
                  <td>Last Update:<br/></td>
                  { 
                    moment(this.props.lastupdate).format("YYYY") > "0001" ? 
                      <td>{ moment(this.props.lastupdate).format("ll") }({moment(this.props.lastupdate).startOf("hour").fromNow()})</td>
                    :
                       <td></td>
                  }
                </tr>
              </tbody>
            </table>*/}
          </div>
        </div>
        {
          this.props.episodeUpdates ?
          <div class="divHalfHorizontal divOverviewUpdates">
            <div>
              <table>
                <tbody>
                  {this.props.episodeUpdates.map((episode) => {
                    return <tr><td><a href={episode.address} target="blank">{episode.episodeNo} {moment(episode.enterDate).format("ll")}</a></td></tr>;
                  })}
                </tbody>
              </table>
            </div>
            <button class="btn btn-success" onClick={this.markAsRead.bind(this)}>Mark as read</button>
          </div>
          :
          ""
        }
      </div>
    );
  }
}