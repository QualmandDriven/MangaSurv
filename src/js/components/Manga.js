import React from "react";

import * as MangaActions from "../actions/MangaActions";

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

  render() {

    const { id, name, chapters, followed, lastupdate, image, newchapters } = this.props;
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
          <span>{ name }</span>
          <p>Chapters: { chapters }</p>
          <p>Last Update: { lastupdate }</p>
        </div>
      </div>
    );
  }
}