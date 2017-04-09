import React, { PropTypes as T } from "react";
import AuthService from '../utils/AuthService'

export default class Profile extends React.Component {
  static contextTypes = {
    router: T.object
  }

  static propTypes = {
    auth: T.instanceOf(AuthService)
  }

  constructor(props, context) {
    super(props, context)
    this.state = {
      profile: props.auth.getProfile(),
      auth: props.auth,
    }
    props.auth.on('profile_updated', (newProfile) => {
      this.setState({profile: newProfile})
    })
  }

  render() {
    const { profile, auth } = this.state;
    console.log(profile);
    return (
      <div>
        <h1>Profile</h1>
        <p>Hi {profile.name}!</p>
        <p>Token: { auth.getToken() }</p>
      </div>
    );
  }
}
