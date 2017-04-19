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

export function getManga(id) {
  console.log("Fetch manga with id " + id)
  dispatcher.dispatch({type: "FETCH_MANGA"});
  fetch('http://192.168.178.70:5000/api/mangas/' + id + "?include=1")
    .then(result => result.json())
    .then(item => dispatcher.dispatch({
    type: "RECEIVE_MANGA", manga: item
  }));
}

export function reloadMangas() {
  dispatcher.dispatch({type: "FETCH_MANGAS"});
  fetch('http://192.168.178.70:5000/api/mangas')
    .then(result => result.json())
    .then(items => dispatcher.dispatch({
    type: "RECEIVE_MANGAS", mangas: items
  }));
}

export function reloadMangasFollowed(user, token) {
  dispatcher.dispatch({type: "FETCH_MANGAS"});
  fetch('http://192.168.178.70:5000/api/users/0/mangas', {
    headers: {'Authorization': 'Bearer ' + localStorage.getItem('id_token') }
  })
    .then(result => result.json())
    .then(items => dispatcher.dispatch({
    type: "RECEIVE_FOLLOWED_MANGAS", mangas: items
  }));
}

export function reloadNewChapters(user, token) {
  dispatcher.dispatch({type: "FETCH_MANGAS"});
  fetch('http://192.168.178.70:5000/api/users/0/chapters?sortby=manga', {
    headers: {'Authorization': 'Bearer ' + localStorage.getItem('id_token') }
    })
    .then(result => result.json())
    .then(items => {
      dispatcher.dispatch({ type: "RECEIVE_NEW_CHAPTERS", mangas: items });
    });
  
}

export function followManga(manga, token) {
  fetch('http://192.168.178.70:5000/api/users/0/mangas', {
    method: 'POST',
    headers: new Headers({
		  'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('id_token')
	  }),
    body: JSON.stringify({
      id: manga.id
    })
  })
  .then(result => {
    reloadMangasFollowed();
    dispatcher.dispatch({type: "FOLLOW_MANGA", manga})
    
  })
  .catch(err => {
    reloadMangasFollowed();
    dispatcher.dispatch({type: "FOLLOW_MANGA", manga});
  });
}

export function unfollowManga(manga, token) {
  fetch('http://192.168.178.70:5000/api/users/0/mangas/' + manga.id, {
    method: 'DELETE',
    headers: {'Authorization': 'Bearer ' + localStorage.getItem('id_token') }
  })
  .then(result => {
    reloadMangasFollowed();
    dispatcher.dispatch({type: "UNFOLLOW_MANGA", manga});
  })
  .catch(err => {
    reloadMangasFollowed();
    dispatcher.dispatch({type: "UNFOLLOW_MANGA", manga});
  });
}

export function markAsRead(manga) {
    manga.chapters.forEach(chapter => {
      fetch('http://192.168.178.70:5000/api/users/0/chapters/' + chapter.id, {
        method: 'DELETE',
        headers: {'Authorization': 'Bearer ' + localStorage.getItem('id_token') }
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
}