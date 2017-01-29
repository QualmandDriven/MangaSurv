import dispatcher from "../dispatcher";

export function createManga(text) {
  dispatcher.dispatch({
    type: "CREATE_MANGA",
    text,
  });
}

export function deleteManga(id) {
  dispatcher.dispatch({
    type: "DELETE_MANGA",
    id,
  });
}

export function reloadMangas() {
  // axios("http://someurl.com/somedataendpoint").then((data) => {
  //   console.log("got the data!", data);
  // })
  
  dispatcher.dispatch({type: "FETCH_MANGAS"});
  fetch('http://192.168.178.70:5000/api/mangas', {
    mode: 'no-cors',
  })
        .then(result => result.json())
        .then(items => dispatcher.dispatch({
        type: "RECEIVE_MANGAS", mangas: items
      }));
}

export function reloadMangasFollowed(user) {
  dispatcher.dispatch({type: "FETCH_MANGAS"});
  // fetch('http://192.168.178.70:5000/api/users/' + user.id + '/mangas')
  fetch('http://192.168.178.70:5000/api/users/' + 1 + '/mangas')
        .then(result => result.json())
        .then(items => dispatcher.dispatch({
        type: "RECEIVE_FOLLOWED_MANGAS", mangas: items
      }));
}

export function reloadNewChapters(user) {
  dispatcher.dispatch({type: "FETCH_MANGAS"});
  // fetch('http://192.168.178.70:5000/api/mangas?chapterstateid=1')
  // fetch('http://192.168.178.70:5000/api/users/' + user.id + '/chapters')
  fetch('192.168.178.70:5000/api/users/1/chapters?sortby=manga', {
    method: 'GET',
    mode: 'no-cors'
    })
    .then(result => result.json())
    .then(items => {
      dispatcher.dispatch({ type: "RECEIVE_NEW_CHAPTERS", mangas: items });
    });
  
}

export function followManga(manga) {
  dispatcher.dispatch({type: "FOLLOW_MANGA", manga});
}

export function unfollowManga(manga) {
  dispatcher.dispatch({type: "UNFOLLOW_MANGA", manga});
}

export function markAsRead(manga) {
  // dispatcher.dispatch({type: "MARKASREAD_MANGA", manga});
    manga.chapters.forEach(chapter => {
      // const formData = new FormData();
      // formData.append('id', chapter.id);
      // console.log(formData);
      fetch('http://192.168.178.70:5000/api/users/1/chapters/' + chapter.id, {
        method: 'DELETE'
        // body: formData
      })
      .then(result => { 
        console.log(result);
        reloadNewChapters();
      })
      .catch(err => { 
        console.log(err);
        reloadNewChapters(); 
      });
    });
    // reloadNewChapters();
}