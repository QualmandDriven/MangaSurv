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

    const imageStyle = {
      height: "170px",
      width: "100%",
    };

    const mangaStyle = {
      width: "10em",
      height: "16em",
      border: 1,
      display: "inline-block",
    }

    const nameStyle = {
      width: "10em",
      height: "3em",
      display: "table-cell",
      verticalAlign: "middle",
      textAlign: "center",
    }

    return (
      <div style={mangaStyle}>
        {followed ? 
          <button class="btn btn-sm btn-danger" onClick={this.unfollowManga.bind(this)}>- Unfollow</button>
          :
          <button class="btn btn-sm btn-primary" onClick={this.followManga.bind(this)}>+ Follow</button> 
        }
        <img src={imagePath} style={imageStyle}/>
        <div style={nameStyle}>
          <table>
            <thead>
              <tr>
                <th>{ name }</th>
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