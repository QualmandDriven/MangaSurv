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
      profile: props.auth.getProfile()
    }
    props.auth.on('profile_updated', (newProfile) => {
      this.setState({profile: newProfile})
    })
  }

  render() {
    const { profile } = this.state;

    return (
      <div>
        <h1>Profile</h1>
        <p>Hi {profile.name}!</p>
      </div>
    );
  }
}
