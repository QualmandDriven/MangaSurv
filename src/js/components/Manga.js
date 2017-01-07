import React from "react";

import * as MangaActions from "../actions/MangaActions";
var moment = require("moment");

export default class Manga extends React.Component {
  constructor(props) {
    super();
  }

  followManga() {
    MangaActions.followManga(this.props);
  }

  unfollowManga() {
    MangaActions.unfollowManga(this.props);
  }

  markAsRead() {
    MangaActions.markAsRead(this.props);
  }

  render() {

    // const { id, name, chapters, followed, lastupdate, image, chapterUpdates } = this.props;
    // const imagePath = "images/" + this.props.image;
    const imagePath = "images/" + this.props.fileSystemName.replace(/[ :]/g, "_") + ".jpg";
    
    return (
      <div class={ this.props.chapterUpdates ? "overview overviewWide" : "overview" }>
        <div class={ this.props.chapterUpdates ? "divHalfHorizontal" : "" }>
          {
            this.props.followed ? 
            <button class="btn btn-sm btn-danger hoverdeleteoverview" onClick={this.unfollowManga.bind(this)}>- Unfollow</button>
            :
            <button class="btn btn-sm btn-primary hoveraddoverview" onClick={this.followManga.bind(this)}>+ Follow</button> 
          }
          <a href="#">
            <img src={imagePath}/>
          </a>
          <div>
            <table>
              <thead>
                <tr>
                  <th colSpan="3">{ this.props.name }</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>Chapters:</td>
                  <td>{ this.props.chapters.length }</td>
                </tr>
                <tr>
                  <td>Last Update:</td>
                  <td>{ moment(this.props.lastupdate).format("ll") }({moment(this.props.lastupdate).startOf("hour").fromNow()})</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        {
          this.props.chapterUpdates ?
          <div class="divHalfHorizontal">
            <div>
              <table>
                <tbody>
                  {this.props.chapterUpdates.map((chapter) => {
                    return <tr key={chapter.id}><td>{chapter.chapterNo}</td><td>{moment(chapter.enterDate).format("ll")}</td></tr>;
                  })}
                </tbody>
              </table>
              <button class="btn btn-success" onClick={this.markAsRead.bind(this)}>Mark as read</button>
            </div>
          </div>
          :
          ""
          
        }
      </div>
    );
  }
}