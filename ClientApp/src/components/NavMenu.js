import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import FeelingLucky from './TravelBot/FeelingLucky';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true,
    };
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed,
    });
  }

  render() {
    return (
      <header>
        <Navbar className='navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-0' container light>
          <NavbarBrand tag={Link} className='font-bold' to='/'>
            <div className='logo'>
              WHERE<span className='logo-ext'>TOGO</span>
            </div>
            <div className='by-kaha'>KAHA TravelBot</div>
          </NavbarBrand>
          <NavbarToggler onClick={this.toggleNavbar} className='mr-2' />
          <Collapse className='d-sm-inline-flex flex-sm-row-reverse' isOpen={!this.state.collapsed} navbar>
            <ul className='navbar-nav flex-grow'>
              <NavItem>
              <FeelingLucky />
              </NavItem>
            </ul>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
