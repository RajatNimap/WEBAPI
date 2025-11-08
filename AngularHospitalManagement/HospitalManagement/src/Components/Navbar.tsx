import logoimage from '../assets/image.png';

export default function Navbar() {
  return (
    <nav className="relative bg-sky-400 after:pointer-events-none after:absolute after:inset-x-0 after:bottom-0 after:h-px after:bg-green-400/20">
      <div className="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
        <div className="relative flex h-16 items-center justify-between">

          {/* Logo + Nav links */}
          <div className="flex flex-1 items-center justify-center sm:items-stretch sm:justify-start">
            <div className="flex shrink-0 items-center ml-5">
              <img
                src={logoimage}
                alt="Your Company"
                className="h-8 w-auto"
              />
            </div>

            {/* Add ml-6 or any value to create spacing */}
            <div className="ml-6">
              <div className="flex space-x-4 text-white">
                <a
                  href="#"
                  className="rounded-md px-3 py-2 text-sm font-medium hover:bg-green-500 hover:text-white"
                >
                  Home
                </a>
                <a
                  href="#"
                  className="rounded-md px-3 py-2 text-sm font-medium hover:bg-green-500 hover:text-white"
                >
                  About
                </a>
              </div>
            </div>
          </div>

          {/* Right side buttons */}
          <div className="absolute inset-y-0 right-0 flex items-center pr-2 sm:static sm:inset-auto sm:ml-6 sm:pr-0">
            <button
              type="button"
              className="relative rounded-full p-1 text-blue-700 hover:text-green-400 focus:outline-2 focus:outline-offset-2 focus:outline-green-400"
            >
              {/* Optional icon */}
            </button>

            {/* Profile dropdown */}
            <div className="relative ml-3">
              <a
                href="#"
                className=" rounded-sm px-4 py-2 text-md font-bold text-white hover:bg-green-400"
              >
                Login
              </a>
            </div>
          </div>
        </div>
      </div>

      {/* Mobile menu dropdown */}
    </nav>
  );
}
