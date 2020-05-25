import React, { useContext } from 'react'
import {
  Button,
  Dropdown,
  Navbar,
  DropdownButton,
  Spinner,
  Container
} from 'react-bootstrap'
import { NavLink, useHistory } from 'react-router-dom'
import GlobalContext from '../../context/GlobalContext'
import * as cookieHandler from '../../helpers/CookieHandler'
import lang from '../../language/SWE/Menu.json'
import ExitBtn from '../broker/ExitBtn'

const Menu = () => {
  const context = useContext(GlobalContext)
  const history = useHistory()

  const handleLogout = () => {
    cookieHandler.deleteSessionCookie()
    context.handleAuthed(false)
    context.handleEmail()
    context.handleOrder({})
    context.handleOrders([])
    context.handleSession('')
    context.handleImages()
    context.handleUser()
    history.push('/login')
  }

  const renderLoading = () => {
    return (
      // <Button variant='primary' disabled>
      <>
        <Spinner
          as='span'
          animation='border'
          size='sm'
          role='status'
          aria-hidden='true'
        />
        <span className='sr-only'>Loading...</span>
      </>
      // { </Button> }
    )
  }

  return (
    <div className='header' bg='light'>
      <Container>
        {context.authed ? (
          <Navbar bg='light' expand='sm'>
            <Navbar.Brand>
              <NavLink to='/orders' style={{ color: 'grey' }}>
                {lang.title}
              </NavLink>
            </Navbar.Brand>
            <Navbar.Toggle aria-controls='basic-navbar-nav' />
            <Navbar.Collapse id='basic-navbar-nav'>
              <DropdownButton
                className='ml-auto'
                id='dropdown-basic-button'
                title={
                  context.userGlobal !== undefined
                    ? context.userGlobal.company
                    : renderLoading()
                }
              >
                <NavLink className='dropdown-item' to='/account'>
                  {lang.accountOverview}
                </NavLink>
                <NavLink className='dropdown-item' to='/pwhelper'>
                  {lang.changePassword}
                </NavLink>

                <NavLink className='dropdown-item' to='' onClick={handleLogout}>
                  {lang.logout}
                </NavLink>
              </DropdownButton>
            </Navbar.Collapse>
          </Navbar>
        ) : context.isBrokerView ? (
          <Navbar bg='light' expand='sm'>
            <Navbar.Brand>
              <NavLink to='/' style={{ color: 'grey' }}>
                {lang.title}
              </NavLink>
            </Navbar.Brand>
            <Navbar.Toggle aria-controls='basic-navbar-nav' />
            <ExitBtn />
          </Navbar>
        ) : (
          <Navbar bg='light' expand='sm'>
            <Navbar.Brand>
              <NavLink to='/' style={{ color: 'grey' }}>
                {lang.title}
              </NavLink>
            </Navbar.Brand>
            <Navbar.Toggle aria-controls='basic-navbar-nav' />
          </Navbar>
        )}
      </Container>
    </div>
  )
}

export default Menu
