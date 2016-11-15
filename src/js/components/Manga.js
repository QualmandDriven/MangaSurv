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

    const { id, name, chapters, followed, lastupdate, image, chapterUpdates } = this.props;
    const imagePath = "images/" + image;

    return (
      <div class="overview">
        {followed ? 
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
                <th colSpan="3">{ name }</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>Chapters:</td>
                <td>{ chapters }</td>
              </tr>
              <tr>
                <td>Last Update:</td>
                <td>{ moment(lastupdate).format("ll") }({moment(lastupdate).startOf("hour").fromNow()})</td>
              </tr>
            </tbody>
          </table>
          {
            chapterUpdates ?
              <div>
                <table>
                  <tbody>
                    {chapterUpdates.map((chapter) => {
                      return <tr key={chapter.id}><td>{chapter.chapter}</td><td>{moment(chapter.added).format("ll")}</td></tr>;
                    })}
                  </tbody>
                </table>
                <button class="btn btn-success" onClick={this.markAsRead.bind(this)}>Mark as read</button>
              </div>
            :
            ""
          }
        </div>
      </div>
    );
  }
}